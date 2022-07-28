using System;
using Controllers;
using Data.UnityObject;
using Data.ValueObject;
using Extentions;
using Keys;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        UnityEngine.Object[] Levels;

        #region Self Variables

        #region Public Variables

        [Header("Data")] public LevelData Data;

        [HideInInspector] public int LevelID => _levelID;

        #endregion

        #region Serialized Variables

        [Space][SerializeField] private GameObject levelHolder;
        [SerializeField] private LevelLoaderCommand levelLoader;
        [SerializeField] private ClearActiveLevelCommand levelClearer;

        #endregion

        #region Private Variables

        [ShowInInspector] private int _levelID;

        #endregion

        #endregion

        protected void Awake()
        {
            _levelID = GetActiveLevel();
            Data = GetLevelData();
            OnInitializeLevel();
        }

        private int GetActiveLevel()
        {
            if (!ES3.FileExists()) return 0;
            return ES3.KeyExists("Level") ? ES3.Load<int>("Level") : 0;
        }

        private LevelData GetLevelData()
        {
            int newLevelData = _levelID % Resources.Load<CD_Level>("Datas/UnityObjects/CD_Level").Levels.Count;
            return Resources.Load<CD_Level>("Datas/UnityObjects/CD_Level").Levels[newLevelData];
        }

        #region Event Subscription

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize += OnInitializeLevel;
            CoreGameSignals.Instance.onClearActiveLevel += OnClearActiveLevel;
            CoreGameSignals.Instance.onNextLevel += OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize -= OnInitializeLevel;
            CoreGameSignals.Instance.onClearActiveLevel -= OnClearActiveLevel;
            CoreGameSignals.Instance.onNextLevel -= OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel -= OnRestartLevel;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        // private void Start()
        // {
        // }

        private void OnNextLevel()
        {
            _levelID++;
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onSaveGameData?.Invoke(new SaveGameDataParams()
            {
                Level = _levelID
            });
            CoreGameSignals.Instance.onLevelInitialize?.Invoke();
        }

        private void OnRestartLevel()
        {
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onSaveGameData?.Invoke(new SaveGameDataParams()
           {
                Level = _levelID
            });
            CoreGameSignals.Instance.onLevelInitialize?.Invoke();
        }

        private void OnInitializeLevel()
        {
             Levels = Resources.LoadAll("Levels");
            var newLevelData = _levelID % Levels.Length;
            levelLoader.InitializeLevel(newLevelData, levelHolder.transform);
            CoreGameSignals.Instance.onCameraInitialized?.Invoke();
        }

        private void OnClearActiveLevel()
        {
            levelClearer.ClearActiveLevel(levelHolder.transform);
        }
    }
}