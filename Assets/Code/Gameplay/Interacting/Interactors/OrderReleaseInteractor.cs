using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Interacting.Interactors
{
    public class OrderReleaseInteractor : MonoBehaviour
    {
        private GameContext _game;

        public void Start()
        {
            _game = Contexts.sharedInstance.game;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.isOrderReleaseZone)
                return false;

            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);
            grabbableEntity.isReleasedAsOrder = true;

            return true;
        }
    }
}