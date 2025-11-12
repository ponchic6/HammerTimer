using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Forge
{
    public class ForgePowerDecreaseSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private readonly CommonStaticData _commonStaticData;

        public ForgePowerDecreaseSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.Forge);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                float totalDecrease = entity.forge.Power * _commonStaticData.forgePowerDecreaseBase * Time.deltaTime;

                entity.forge.Power = Mathf.Max(0f, entity.forge.Power - totalDecrease);
            }
        }
    }
}