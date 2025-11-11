using Code.Gameplay.Produce.Anvil;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Interacting.Interactors
{
    public class AnvilInteractor : MonoBehaviour
    {
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;      
        }

        public bool TryInteractWithoutItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasAnvil)
                return false;

            if (!socketEntity.hasProduceProgress && !socketEntity.hasGrabbedItem)
            {
                Anvil anvilComponent = socketEntity.anvil;
                int currentIndex = anvilComponent.PossibleItems.IndexOf(anvilComponent.CurrentProduceItem);
                int nextIndex = (currentIndex + 1) % anvilComponent.PossibleItems.Count;
                anvilComponent.CurrentProduceItem = anvilComponent.PossibleItems[nextIndex];
                return true;
            }

            if (socketEntity.hasGrabbedItem)
            {
                int grabbableId = socketEntity.grabbedItem.Value;
                playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
                socketEntity.RemoveGrabbedItem();
                return true;
            }
            
            return false;
        }

        public bool TryInteractWithItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasAnvil || socketEntity.hasProduceProgress || socketEntity.hasGrabbedItem || socketEntity.anvil.CurrentProduceItem == ItemsEnum.NoItem)
                return false;
            
            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);

            if (grabbableEntity.grabbableItem.Value != ItemsEnum.IronIngot || !grabbableEntity.hasGrabbableTemperature)
                return false;

            socketEntity.AddProduceProgress(0f, socketEntity.anvil.CurrentProduceItem);
            socketEntity.AddAnvilQuality(0f, grabbableEntity.grabbableTemperature.Value);
            playerEntityBehavior.Entity.RemoveGrabbedItem();
            grabbableEntity.isDestructed = true;

            return true;
        }
    }
}
