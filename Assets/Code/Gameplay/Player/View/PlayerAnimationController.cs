using UnityEngine;

namespace Code.Gameplay.Player.View
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private static readonly int IdleTrigger = Animator.StringToHash("IdleTrigger");
        private static readonly int RunTrigger = Animator.StringToHash("RunTrigger");
        private int _currentAnimationState = IdleTrigger;

        public void TrySetIdleState()
        {
            if (_currentAnimationState == IdleTrigger)
                return;
            animator.SetTrigger(IdleTrigger);
            _currentAnimationState = IdleTrigger;
        }

        public void TrySetRunState()    
        {
            if (_currentAnimationState == RunTrigger)
                return;
            animator.SetTrigger(RunTrigger);
            _currentAnimationState = RunTrigger;
        }
    }
}