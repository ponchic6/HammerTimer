using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Workbench
{
    public class WorkbenchProgressIncreaseSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public WorkbenchProgressIncreaseSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProducingByPlayer, GameMatcher.Workbench, GameMatcher.ProduceProgress));
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