using System.Collections.Generic;
using Code.Gameplay.Grabbing;
using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Systems
{
    public class IronAggregationStateSystem : IExecuteSystem
    {
        private readonly CommonStaticData _commonStaticData;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(32);

        public IronAggregationStateSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;
            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.GrabbableItem, GameMatcher.GrabbableTemperature));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.grabbableItem.Value != ItemsEnum.MoltenIron && entity.grabbableItem.Value != ItemsEnum.IronIngot)
                    continue;

                if (entity.grabbableTemperature.Value <= _commonStaticData.meltingTemperature)
                {
                    if (entity.grabbableItem.Value == ItemsEnum.MoltenIron)
                        entity.grabbableItem.Value = ItemsEnum.IronIngot;
                }
                else
                {
                    if (entity.grabbableItem.Value == ItemsEnum.IronIngot)
                        entity.grabbableItem.Value = ItemsEnum.MoltenIron;
                }
            }
        }
    }
}