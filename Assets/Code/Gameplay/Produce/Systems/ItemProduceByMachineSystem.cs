using System.Collections.Generic;
using Code.Gameplay.Grabbing.Services;
using Entitas;

namespace Code.Gameplay.Produce.Systems
{
    public class ItemProduceByMachineSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(16);

        public ItemProduceByMachineSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProduceProgress, GameMatcher.ProduceMachine, GameMatcher.GrabbedItem));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.produceProgress.Value < 1f)
                    continue;
                
                entity.RemoveProduceProgress();
                entity.ReplaceGrabbedItem(entity.produceMachine.To);
            }
        }
    }
}