using Code.Gameplay.Grabbing;
using Code.Gameplay.Grabbing.Registrars;
using Code.Gameplay.Grabbing.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player
{
    public class PlayerFreeItemGrabber : MonoBehaviour
    {
        private IGrabbableFactory _grabbableFactory;

        [Inject]
        public void Construct(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
        }

        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, LayerMask grabbableLayer, Collider[] overlapResults)
        {
            Vector3 position = transform.position + transform.forward;

            int grabbableCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, grabbableLayer);

            if (grabbableCount <= 0)
                return false;
            
            GrabbableRegistrar itemToGrab = overlapResults[0].GetComponent<GrabbableRegistrar>();
            GameEntity grabbaleEntity = itemToGrab.GetComponent<EntityBehaviour>().Entity;
                
            if (!grabbaleEntity.hasGrabbableItem)
                return false;
                
            playerEntityBehavior.Entity.AddGrabbedItem(itemToGrab.grabbableEnum);
            grabbaleEntity.isDestructed = true;

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            GrabbableEnum grabbableEnum = playerEntityBehavior.Entity.grabbedItem.Value;
            
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);

            if (socketCount > 0)
                return false;
            
            _grabbableFactory.SpawnNearWithPlayer(grabbableEnum);
            playerEntityBehavior.Entity.RemoveGrabbedItem();

            return true;
        } 
    }
}