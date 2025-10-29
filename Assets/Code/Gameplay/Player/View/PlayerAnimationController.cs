using UnityEngine;

namespace Code.Gameplay.Player.View
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int Blend = Animator.StringToHash("Blend");
        [SerializeField] private Animator animator;

        public void SetBlendMotion(float newValue)
        {
            animator.SetFloat(Blend, newValue);
        }
    }
}