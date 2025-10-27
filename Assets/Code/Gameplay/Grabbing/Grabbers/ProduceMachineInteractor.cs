using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Grabbing.Grabbers
{
    public class ProduceMachineInteractor : MonoBehaviour
    {
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;       
        }

        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasProduceMachine || !socketEntity.hasGrabbedItem || socketEntity.hasProduceProgress)
                return false;

            int grabbableId = socketEntity.grabbedItem.Value;
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
            socketEntity.RemoveGrabbedItem();

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasProduceMachine || socketEntity.hasGrabbedItem)
                return false;

            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);

            if (socketEntity.produceMachine.From != grabbableEntity.grabbableItem.Value)
                return false;

            socketEntity.AddProduceProgress(0f, socketEntity.produceMachine.To);
            playerEntityBehavior.Entity.RemoveGrabbedItem();
            grabbableEntity.isDestructed = true;

            return true;
        }
    }
}