using Code.Gameplay.Player.View;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Player.Registrars
{
    public class PlayerAnimatorRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private PlayerAnimationController playerAnimationController;
    
        public override void RegisterComponent() =>
            Entity.AddPlayerAnimator(playerAnimationController);

        public override void UnregisterComponent() =>
            Entity.RemovePlayerAnimator();
    }
}