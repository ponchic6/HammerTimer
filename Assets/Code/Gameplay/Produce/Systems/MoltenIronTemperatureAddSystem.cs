using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.StaticData;
using Entitas;

namespace Code.Gameplay.Produce.Systems
{
    public class IronTemperatureAddSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(32);
        private readonly CommonStaticData _commonStaticData;

        public IronTemperatureAddSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.GrabbableItem).NoneOf(GameMatcher.GrabbableTemperature));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.grabbableItem.Value != ItemsEnum.IronIngot)
                    continue;
            
                entity.AddGrabbableTemperature(_commonStaticData.environmentTemperature);
            }
        }
    }
}