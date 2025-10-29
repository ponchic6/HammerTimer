using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Interacting.Interactors
{
    public class ShelfSocketInteractor : MonoBehaviour
    {
        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.isShelf || !socketEntity.hasGrabbedItem)
                return false;

            int grabbableId = socketEntity.grabbedItem.Value;
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
            socketEntity.RemoveGrabbedItem();

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.isShelf || socketEntity.hasGrabbedItem)
                return false;

            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            socketEntity.AddGrabbedItem(grabbableId);
            playerEntityBehavior.Entity.RemoveGrabbedItem();

            return true;
        }
    }
}