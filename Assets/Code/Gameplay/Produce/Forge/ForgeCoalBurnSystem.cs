using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Forge
{
    public class ForgeCoalBurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private readonly CommonStaticData _commonStaticData;

        public ForgeCoalBurnSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.Forge);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if (entity.forge.Coal > 0)
                {
                    entity.forge.Coal = Mathf.Max(0, entity.forge.Coal - Time.deltaTime);
                    entity.forge.Temperature = Mathf.Min(_commonStaticData.forgeMaxTemperature, entity.forge.Temperature + _commonStaticData.forgeTemperatureIncreaseRate * Time.deltaTime);
                }
                else
                {
                    entity.forge.Temperature = Mathf.Max(25, entity.forge.Temperature - _commonStaticData.forgeTemperatureDecreaseRate * Time.deltaTime);
                }
            }
        }
    }
}