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

            MoldEnum moldEnumValue = socketEntity.mouldingMachine.MoldEnum;
            
            if (moldEnumValue != MoldEnum.NoMold && !socketEntity.hasMouldingQuality)
            {
                Enum.TryParse(socketEntity.mouldingMachine.MoldEnum.ToString(), out ItemsEnum moldItemEnum);
                GameEntity moldEntity = _grabbableFactory.SpawnAtPosition(moldItemEnum, transform.position, false, socketEntity.mouldingMachine.MoldEnum, socketEntity.mouldingMachine.Item);
                playerEntityBehavior.Entity.AddGrabbedItem(moldEntity.id.Value);
                socketEntity.mouldingMachine.MoldEnum = MoldEnum.NoMold;
                socketEntity.mouldingMachine.Item = ItemsEnum.NoItem;
                return true;
            }
            
            if (!socketEntity.hasGrabbedItem)
                return false;

            int grabbableId = socketEntity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);
            grabbableEntity.AddQuality(socketEntity.mouldingQuality.Quality);
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableId);
            socketEntity.RemoveMouldingQuality();
            socketEntity.RemoveGrabbedItem();
            socketEntity.mouldingMachine.MoldEnum = MoldEnum.NoMold;
            socketEntity.mouldingMachine.Item = ItemsEnum.NoItem;

            return true;
        }

        public bool TryReleaseItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            int grabbableId = playerEntityBehavior.Entity.grabbedItem.Value;
            GameEntity grabbableEntity = _game.GetEntityWithId(grabbableId);
            
            if (!socketEntity.hasMouldingMachine || (grabbableEntity.grabbableItem.Value != ItemsEnum.MoltenIron && !grabbableEntity.hasMold))
                return false; ;
            
            if (grabbableEntity.hasMold && socketEntity.mouldingMachine.MoldEnum == MoldEnum.NoMold && grabbableEntity.mold.Mold != MoldEnum.NoMold)
            {
                socketEntity.mouldingMachine.MoldEnum = grabbableEntity.mold.Mold;
                socketEntity.mouldingMachine.Item = grabbableEntity.mold.Item;
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                grabbableEntity.isDestructed = true;
                return true;
            }

            if (grabbableEntity.grabbableItem.Value == ItemsEnum.MoltenIron && socketEntity.mouldingMachine.MoldEnum != MoldEnum.NoMold)
            {
                GameEntity producedEntity = _grabbableFactory.SpawnAtPosition(socketEntity.mouldingMachine.Item, transform.position, false);
                socketEntity.AddMouldingQuality(0f, Time.time);
                socketEntity.AddGrabbedItem(producedEntity.id.Value);
                playerEntityBehavior.Entity.RemoveGrabbedItem();
                grabbableEntity.isDestructed = true;
                return true;
            }

            return false;

        }
    }
}