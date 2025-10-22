using Code.Gameplay.Grabbing;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Player
{
    public class PlayerShelfSocketGrabber : MonoBehaviour
    {
        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);

            if (socketCount <= 0)
                return false;
            
            Collider socketCollider = overlapResults[0];
            GameEntity socketEntity = socketCollider.GetComponent<EntityBehaviour>().Entity;

            if (!socketEntity.isSocket || socketEntity.hasProduceMachine || !socketEntity.hasGrabbedItem)
                return false;

            GrabbableEnum grabbableEnum = socketEntity.grabbedItem.Value;
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableEnum);
            socketEntity.RemoveGrabbedItem();

            return true;
        }
        
        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            GrabbableEnum grabbableEnum = playerEntityBehavior.Entity.grabbedItem.Value;
            
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);
        
            if (socketCount <= 0)
                return false;
        
            GameEntity socketEntity = overlapResults[0].GetComponent<EntityBehaviour>().Entity;

            if (!socketEntity.isSocket || socketEntity.hasProduceMachine || socketEntity.hasGrabbedItem) 
                return false;
            
            socketEntity.AddGrabbedItem(grabbableEnum);
            playerEntityBehavior.Entity.RemoveGrabbedItem();

            return true;
        }
    }
}