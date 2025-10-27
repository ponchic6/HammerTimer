using System.Collections.Generic;
using Code.Gameplay.Grabbing.Services;
using Entitas;

namespace Code.Gameplay.Produce.Systems
{
    public class ProduceMachineProduceSystem : IExecuteSystem
    {
        private readonly IGrabbableFactory _grabbableFactory;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(16);

        public ProduceMachineProduceSystem(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProduceProgress, GameMatcher.ProduceMachine));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.produceProgress.Progress < 1f)
                    continue;

                GameEntity grabbableEntity = _grabbableFactory.SpawnAtPosition(entity.produceProgress.Item, entity.transform.Value.position, false);
                entity.AddGrabbedItem(grabbableEntity.id.Value);
                entity.RemoveProduceProgress();
            }
        }
    }
}