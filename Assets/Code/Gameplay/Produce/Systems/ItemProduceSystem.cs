using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Produce.Systems
{
    public class ItemProduceSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(16);

        public ItemProduceSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProduceProgress));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.produceProgress.Progress < 1f)
                    continue;

                entity.ReplaceGrabbedItem(entity.produceProgress.Item);
                entity.RemoveProduceProgress();
            }
        }
    }
}