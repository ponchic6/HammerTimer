using System;
using Code.Gameplay.Grabbing.Services;
using Code.Gameplay.Produce;
using Code.Gameplay.Produce.Moulding;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Grabbing.Grabbers
{
    public class MouldingMachineInteractor : MonoBehaviour
    {
        private IGrabbableFactory _grabbableFactory;
        private GameContext _game;

        [Inject]
        public void Construct(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
            _game = Contexts.sharedInstance.game;
        }

        
        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasMouldingMachine || !socketEntity.hasGrabbedItem || !socketEntity.hasMouldingQuality)
                return false;

            int grabbableId = socketEntity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);
            grabbableEntity.AddQuality(socketEntity.mouldingQuality.Quality);
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
            socketEntity.RemoveMouldingQuality();
            socketEntity.RemoveGrabbedItem();
            socketEntity.mouldingMachine.MoldEnumValue = MoldEnum.NoMold;

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);
            
            if (!socketEntity.hasMouldingMachine || (grabbableEntity.grabbableItem.Value != ItemsEnum.MoltenIron && !grabbableEntity.hasMold))
                return false;

            MouldingMachineComponent mouldingMachine = socketEntity.mouldingMachine;
            
            if (grabbableEntity.hasMold && mouldingMachine.MoldEnumValue == MoldEnum.NoMold && grabbableEntity.mold.Value != MoldEnum.NoMold)
            {
                mouldingMachine.MoldEnumValue = grabbableEntity.mold.Value;
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                grabbableEntity.isDestructed = true;
                return true;
            }

            if (grabbableEntity.grabbableItem.Value == ItemsEnum.MoltenIron && mouldingMachine.MoldEnumValue != MoldEnum.NoMold)
            {
                Enum.TryParse(mouldingMachine.MoldEnumValue.ToString(), out ItemsEnum itemEnum);
                GameEntity producedEntity = _grabbableFactory.SpawnAtPosition(itemEnum, transform.position, false);
                socketEntity.AddMouldingQuality(0f, DateTime.Now.TimeOfDay);
                socketEntity.AddGrabbedItem(producedEntity.id.Value);
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                grabbableEntity.isDestructed = true;
                return true;
            }

            return false;

        }
    }
}