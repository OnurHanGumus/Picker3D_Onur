using System;
using Managers;
using UnityEngine;
using Enums;

namespace Controllers
{
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationStates _animationStates;
        
        #endregion
        #region private vars
        #endregion

        #endregion

        private void Awake()
        {
            StartIdleAnim();
        }

        #region EventSubsicription
        private void Start()
        {
            SubscribeEvents();
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onPlay += StartRunAnim;
            CoreGameSignals.Instance.onLevelSuccessful += StartFinishAnim;
        }
        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.onPlay -= StartRunAnim;
            CoreGameSignals.Instance.onLevelSuccessful -= StartFinishAnim;
        }
        #endregion

        #region Animation State Change

        private void ChangeAnimationData(AnimationStates animationStates)
        {
            _animationStates = animationStates;
        }
        
        private void StartIdleAnim()
        {
            ChangeAnimationData(AnimationStates.Idle);
            ResetAllAnims();
            _animator.SetBool("Idle",true);
        }
        
        private void StartRunAnim()
        {
            ChangeAnimationData(AnimationStates.Run);
            ResetAllAnims();
            _animator.SetBool("Run",true);
        }
        
        private void StartFinishAnim()
        {
            ChangeAnimationData(AnimationStates.Finish);
        }

        private void ResetAllAnims()
        {
            _animator.SetBool("Idle" ,false);
            _animator.SetBool("Run" ,false);
        }
        #endregion
    }
}