using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Produce.Systems
{
    public class WorkbenchProduceSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(4);

        public WorkbenchProduceSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProduceProgress, GameMatcher.Workbench).NoneOf(GameMatcher.GrabbedItem));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.produceProgress.Progress < 1f)
                    continue;

                entity.AddGrabbedItem(entity.produceProgress.Item);
                entity.RemoveProduceProgress();
                entity.workbench.Value.Clear();
            }
        }
    }
}