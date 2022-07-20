using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using Signals;

public class CollectableManager : MonoBehaviour
{
    #region Self Variables
    #region public vars
    public CollectableType collectableType;
    public CollectableState collectableState = CollectableState.notCollected;
    public Transform connectedNode;

    #endregion
    #region Serialized Variables

    #endregion
    #region private var
    private MeshRenderer _meshRenderer;
    private CollectableMovementController _collectableMovementController;
    private CollectableData _collectableData;
    private CollectableStackManager _collectableStackManager;

    #endregion
    #endregion

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collectableMovementController = GetComponent<CollectableMovementController>();
        SendCollectableDataToController();
        collectableType = GetCollectableType();
        ChangeMeshRenderer();

        _collectableStackManager = FindObjectOfType<CollectableStackManager>();

    }

    void Start()
    {
        SubscribeEvents();
        //_connectedNode = _collectableStackManager.GetLastNodeOfList(transform);

    }

    private void SubscribeEvents()
    {
        CollectableSignals.Instance.onCollectableAndObstacleCollide += OnCollectableAndObstacleCollide;
        CollectableSignals.Instance.onCollectableAndCollectableCollide += OnCollectableAndCollectableCollide;
        CollectableSignals.Instance.onCollectableUpgradeCollide += OnUpgradeCollectableCollide;
        CollectableSignals.Instance.onCollectableATMCollide += OnCollectableAndATMCollide;
        PlayerSignals.Instance.onPlayerAndObstacleCrash += OnPlayerAndObstacleCrash;

        CollectableSignals.Instance.onGetCollectableType += OnGetCollectableType;
        
    }
    private void UnsubscribeEvents()
    {
        CollectableSignals.Instance.onCollectableAndObstacleCollide -= OnCollectableAndObstacleCollide;
        CollectableSignals.Instance.onCollectableAndCollectableCollide -= OnCollectableAndCollectableCollide;
        CollectableSignals.Instance.onCollectableUpgradeCollide -= OnUpgradeCollectableCollide;
        CollectableSignals.Instance.onCollectableATMCollide -= OnCollectableAndATMCollide;
        PlayerSignals.Instance.onPlayerAndObstacleCrash -= OnPlayerAndObstacleCrash;

        CollectableSignals.Instance.onGetCollectableType -= OnGetCollectableType;

    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void ChangeMeshRenderer()
    {
        _meshRenderer.material = GetMaterial();

    }
    private Material GetMaterial() => Resources.Load<Material>("Materials/" + collectableType.ToString());
    private CollectableData GetCollectableData() => Resources.Load<CD_Collectable>("Datas/UnityObjects/CD_Collectable").Data;
    private CollectableType GetCollectableType() => Resources.Load<CD_Collectable>("Datas/UnityObjects/CD_Collectable").collectableType;

    public void OnUpgradeCollectableCollide(Transform upgradedNode)
    {
        if (upgradedNode.Equals(transform))
        {
            ++collectableType;
            ChangeMeshRenderer();
        }   
    }

    private void OnCollectableAndObstacleCollide(Transform crashedNode)
    {
        if (collectableState.Equals(CollectableState.collected))
        {
            if (crashedNode.localPosition.z <= transform.localPosition.z)
            {
                collectableState = CollectableState.notCollected;
                CollectableBreak();
            }
        }
    }

    private void OnPlayerAndObstacleCrash()
    {
        if (collectableState.Equals(CollectableState.collected))
        {
            collectableState = CollectableState.notCollected;
            CollectableBreak();
        }
    }

    private void CollectableBreak()
    {
        transform.tag = "Collectable";

        _collectableMovementController.DeactivateMovement();
        _collectableMovementController.AddDropForce();
    }

    private void OnCollectableAndCollectableCollide(Transform otherNode, Transform parent)
    {
        if (otherNode == transform)
        {
            collectableState = CollectableState.collected;

            //Transform parentNode = _collectableStackManager.GetLastNodeOfList();
            //_collectableMovementController.SetConnectedNode(parentNode);

            _collectableMovementController.ActivateMovement();


        }
    }

    private void SendCollectableDataToController()
    {
        _collectableData = GetCollectableData();
        _collectableMovementController.SetCollectableData(_collectableData);
    }

    private void OnCollectableAndATMCollide(Transform atmyeGirenObje)
    {
        if (atmyeGirenObje.Equals(transform))
        {        
            ScoreSignals.Instance.onATMScoreUpdated?.Invoke((int)collectableType);
            DestroyCollectable();
        }

        else if (collectableState.Equals(CollectableState.collected))
        {
            if (atmyeGirenObje.localPosition.z <= transform.localPosition.z)
            {
                collectableState = CollectableState.notCollected;
                CollectableBreak();
            }
            //DestroyCollectable();
        }

        
    }

    private void DestroyCollectable()
    {
        _collectableMovementController.DeactivateMovement();
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private int OnGetCollectableType()
    {

        if (collectableState.Equals(CollectableState.notCollected))
        {
            print(collectableType);

        }
        return (int) collectableType;
    }

    public void SetControllerParentNode(Transform parentNode)
    {
        _collectableMovementController.SetConnectedNode(parentNode);
    }
}
