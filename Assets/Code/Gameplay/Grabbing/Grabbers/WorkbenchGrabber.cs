using Code.Gameplay.Grabbing.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Grabbing.Grabbers
{
    public class WorkbenchGrabber : MonoBehaviour
    {
        private IGrabbableFactory _grabbableFactory;

        [Inject]
        public void Construct(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
        }

        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);

            if (socketCount <= 0)
                return false;
            
            Collider socketCollider = overlapResults[0];
            GameEntity socketEntity = socketCollider.GetComponent<EntityBehaviour>().Entity;

            if (!socketEntity.hasWorkbench || !socketEntity.hasGrabbedItem)
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

            if (!socketEntity.hasWorkbench) 
                return false;

            playerEntityBehavior.Entity.RemoveGrabbedItem();
            socketEntity.workbench.Value.Add(grabbableId);
            return true;
        }

        public bool TryClearWorkbench(float interactionRadius, LayerMask socketLayer, Collider[] overlapResults)
        {
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, interactionRadius, overlapResults, socketLayer);

            if (socketCount <= 0)
                return false;

            GameEntity socketEntity = overlapResults[0].GetComponent<EntityBehaviour>().Entity;

            if (!socketEntity.hasWorkbench || socketEntity.workbench.Value.Count == 0)
                return false;

            Vector3 workbenchPosition = socketEntity.transform.Value.position;

            foreach (string grabbableId in socketEntity.workbench.Value)
            {
                Vector3 spawnPosition = workbenchPosition + transform.right * Random.Range(-1f, 1f) + transform.forward * 1.5f;
                _grabbableFactory.SpawnAtPosition(grabbableId, spawnPosition);
            }

            socketEntity.workbench.Value.Clear();

            if (socketEntity.hasProduceProgress)
                socketEntity.RemoveProduceProgress();

            return true;
        }
    }
}