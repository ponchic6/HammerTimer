using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Anvil
{
    public class CoolingOnAnvilSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private CommonStaticData _commonStaticData;

        public CoolingOnAnvilSystem(CommonStaticData commonStaticData)
        {
            _game = Contexts.sharedInstance.game;
            _commonStaticData = commonStaticData;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Anvil, GameMatcher.AnvilQuality));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if (entity.anvilQuality.Temperature <= _commonStaticData.environmentTemperature)
                    continue;
                
                entity.anvilQuality.Temperature = Mathf.Max(_commonStaticData.environmentTemperature, entity.anvilQuality.Temperature - _commonStaticData.itemCoolingRate * Time.deltaTime);
            }
        }
    }
}