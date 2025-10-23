using Code.Gameplay.Grabbing.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Grabbing.Grabbers
{
    public class FreeItemGrabber : MonoBehaviour
    {
        private IGrabbableFactory _grabbableFactory;

        [Inject]
        public void Construct(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
        }

        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask grabbableLayer, Collider[] overlapResults)
        {
            Vector3 position = transform.position + transform.forward;

            int grabbableCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, grabbableLayer);

            if (grabbableCount <= 0)
                return false;
            
            GameEntity grabbableEntity = overlapResults[0].GetComponent<EntityBehaviour>().Entity;
                
            if (!grabbableEntity.hasGrabbableItem)
                return false;
                
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableEntity.grabbableItem.Value);
            grabbableEntity.isDestructed = true;

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            string grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);

            if (socketCount > 0)
                return false;
            
            _grabbableFactory.SpawnNearWithPlayer(grabbableId);
            playerEntityBehavior.Entity.RemoveGrabbedItem();

            return true;
        } 
    }
}