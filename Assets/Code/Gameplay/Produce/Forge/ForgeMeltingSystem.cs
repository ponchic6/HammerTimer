using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Forge
{
    public class TemperatureIncreaseByForgeSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly GameContext _game;
        private readonly CommonStaticData _commonStaticData;

        public TemperatureIncreaseByForgeSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Forge, GameMatcher.GrabbedItem));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                GameEntity grabbableEntity = _game.GetEntityWithId(entity.grabbedItem.Value);

                float targetTemperature = entity.forge.Temperature;
                float currentTemperature = grabbableEntity.grabbableTemperature.Value;
                grabbableEntity.grabbableTemperature.Value = Mathf.MoveTowards(currentTemperature, targetTemperature, _commonStaticData.itemTemperatureChangeRate * Time.deltaTime);
            }
        }
    }
}