using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Anvil
{
    public class ProgressAndQualityForgingSystem : IExecuteSystem
    {
        private readonly CommonStaticData _commonStaticData;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public ProgressAndQualityForgingSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Anvil, GameMatcher.AnvilQuality, GameMatcher.ProduceProgress, GameMatcher.ProducingByPlayer));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.produceProgress.Progress += Time.deltaTime * 0.3f;
                entity.anvilQuality.Quality += 
                    (1f - Mathf.Abs(_commonStaticData.temperatureForMaxForgingQuality - entity.anvilQuality.Temperature) / _commonStaticData.temperatureForMaxForgingQuality) * Time.deltaTime * 0.3f;
            }
        }
    }
}