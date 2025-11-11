using System;
using System.Collections.Generic;
using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.View;
using Entitas;

namespace Code.Gameplay.Produce.Systems
{
    public class AddMoldToClayFormSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(16);

        public AddMoldToClayFormSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.GrabbableItem).NoneOf(GameMatcher.Mold));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (!ShouldHasMold(entity))
                    continue;

                AddMold(entity);
            }
        }

        private static void AddMold(GameEntity entity)
        {
            Enum.TryParse(entity.grabbableItem.Value.ToString(), out MoldEnum moldEnum);

            ItemsEnum itemEnum = ItemsEnum.NoItem;
                
            switch (moldEnum)
            {
                case MoldEnum.AxeBladeMold:
                    itemEnum = ItemsEnum.AxeBlade;
                    break;
                case MoldEnum.SwordBladeMold:
                    itemEnum = ItemsEnum.SwordBlade;
                    break;
                case MoldEnum.IngotMold:
                    itemEnum = ItemsEnum.IronIngot;
                    break;
            }
                
            entity.AddMold(moldEnum, itemEnum);
        }

        private bool ShouldHasMold(GameEntity entity)
        {
            ItemsEnum itemsEnum = entity.grabbableItem.Value;
            return itemsEnum is ItemsEnum.AxeBladeMold or ItemsEnum.SwordBladeMold or ItemsEnum.IngotMold;
        }
    }
}