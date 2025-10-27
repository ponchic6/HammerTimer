using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Systems
{
    public class ProduceIncreaseByPlayerSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public ProduceIncreaseByPlayerSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProducingByPlayer, GameMatcher.ProduceProgress));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.produceProgress.Progress += Time.deltaTime * 0.3f;
            }
        }
    }
}