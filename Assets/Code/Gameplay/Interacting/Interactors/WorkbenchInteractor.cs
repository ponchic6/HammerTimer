using Code.Infrastructure.View;
using Code.Infrastructure.View.Extensions;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Interacting.Interactors
{
    public class WorkbenchInteractor : MonoBehaviour
    {
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;       
        }

        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasWorkbench || !socketEntity.hasGrabbedItem)
                return false;

            int grabbableId = socketEntity.grabbedItem.Value;
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
            socketEntity.RemoveGrabbedItem();

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasWorkbench)
                return false;

            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);

            socketEntity.workbench.Value.Add(grabbableEntity.id.Value);
            playerEntityBehavior.Entity.RemoveGrabbedItem();
            return true;
        }

        public bool TryClearWorkbench(GameEntity socketEntity)
        {
            if (!socketEntity.hasWorkbench || socketEntity.workbench.Value.Count == 0)
                return false;

            Vector3 workbenchPosition = socketEntity.transform.Value.position;

            foreach (int grabbableId in socketEntity.workbench.Value)
            {
                Vector3 spawnPosition = workbenchPosition + transform.right * Random.Range(-1f, 1f) + transform.forward * 1.5f;
                GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);
                grabbableEntity.AddInitialTransform(spawnPosition, Quaternion.identity);
                grabbableEntity.EnableView();
            }

            socketEntity.workbench.Value.Clear();

            if (socketEntity.hasProduceProgress)
                socketEntity.RemoveProduceProgress();

            return true;
        }
    }
}