using System;
using Code.Gameplay.Interacting.Services;
using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Interacting.Interactors
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
            if (!socketEntity.hasMouldingMachine)
                return false;

            MoldEnum moldEnumValue = socketEntity.mouldingMachine.MoldEnumValue;
            
            if (moldEnumValue != MoldEnum.NoMold && !socketEntity.hasMouldingQuality)
            {
                GameEntity moldEntity = _grabbableFactory.SpawnAtPosition(socketEntity.mold.Item, transform.position, false, socketEntity.mold.Mold);
                playerEntityBehavior.Entity.AddGrabbedItem(moldEntity.id.Value);
                socketEntity.mouldingMachine.MoldEnumValue = MoldEnum.NoMold;
                return true;
            }

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
                return false; ;
            
            if (grabbableEntity.hasMold && socketEntity.mouldingMachine.MoldEnumValue == MoldEnum.NoMold && grabbableEntity.mold.Mold != MoldEnum.NoMold)
            {
                socketEntity.mouldingMachine.MoldEnumValue = grabbableEntity.mold.Mold;
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                grabbableEntity.isDestructed = true;
                return true;
            }

            if (grabbableEntity.grabbableItem.Value == ItemsEnum.MoltenIron && socketEntity.mouldingMachine.MoldEnumValue != MoldEnum.NoMold)
            {
                Enum.TryParse(socketEntity.mouldingMachine.MoldEnumValue.ToString(), out ItemsEnum item);
                GameEntity producedEntity = _grabbableFactory.SpawnAtPosition(item, transform.position, false);
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