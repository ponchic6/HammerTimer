using Code.Infrastructure.View;
using Code.Infrastructure.View.Extensions;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Interacting.Interactors
{
    public class FreeItemInteractor : MonoBehaviour
    {
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;
        }

        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, GameEntity targetEntity)
        {
            if (!targetEntity.hasGrabbableItem)
                return false;

            playerEntityBehavior.Entity.AddGrabbedItem(targetEntity.id.Value);
            targetEntity.DisableView();

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior)
        {
            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity entity = _game.GetEntityWithId(grabbableId);
            Vector3 spawnPosition = playerEntityBehavior.transform.position + playerEntityBehavior.transform.forward;
            entity.AddInitialTransform(spawnPosition, Quaternion.identity);
            entity.AddViewState(true);
            entity.rigidbody.Value.linearVelocity = Vector3.zero;
            playerEntityBehavior.Entity.RemoveGrabbedItem();

            return true;
        }
    }
}