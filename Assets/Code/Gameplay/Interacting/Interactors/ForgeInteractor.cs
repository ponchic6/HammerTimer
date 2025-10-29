using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Interacting.Interactors
{
    public class ForgeInteractor : MonoBehaviour
    {
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;
        }
        
        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasForge || !socketEntity.hasGrabbedItem)
                return false;

            int grabbableId = socketEntity.grabbedItem.Value;
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
            socketEntity.RemoveGrabbedItem();

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasForge || socketEntity.hasProduceProgress)
                return false;

            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);

            ItemsEnum grabbableItemType = grabbableEntity.grabbableItem.Value;
            
            if (grabbableItemType == ItemsEnum.Coal)
            {
                socketEntity.forge.Coal += 30f;
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                grabbableEntity.isDestructed = true;
                return true;
            }

            if (grabbableEntity.hasGrabbableTemperature && grabbableItemType is ItemsEnum.IronIngot or ItemsEnum.MoltenIron)
            {
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                socketEntity.AddGrabbedItem(grabbableId);
            }

            return true;
        }
    }
}