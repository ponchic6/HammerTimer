using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Grabbing.Grabbers
{
    public class ForgeGrabber : MonoBehaviour
    {
        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);

            if (socketCount <= 0)
                return false;
            
            Collider socketCollider = overlapResults[0];
            GameEntity socketEntity = socketCollider.GetComponent<EntityBehaviour>().Entity;

            if (!socketEntity.hasForge || !socketEntity.hasGrabbedItem)
                return false;

            string grabbableId = socketEntity.grabbedItem.Value;
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
            socketEntity.RemoveGrabbedItem();

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            string grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);
        
            if (socketCount <= 0)
                return false;
        
            GameEntity socketEntity = overlapResults[0].GetComponent<EntityBehaviour>().Entity;

            if (!socketEntity.hasForge || socketEntity.hasProduceProgress) 
                return false;

            if (grabbableId == "GrabbableItems/Coal")
            {
                socketEntity.forge.Coal += 30f;
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                return true;
            }

            if (grabbableId == "GrabbableItems/Iron" && !socketEntity.hasProduceProgress)
            {
                socketEntity.AddProduceProgress(0f, "GrabbableItems/MoltenIron");
                playerEntityBehavior.Entity.RemoveGrabbedItem();
            }
            
            return true;
        }
    }
}