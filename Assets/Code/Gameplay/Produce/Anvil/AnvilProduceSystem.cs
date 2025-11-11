using System.Collections.Generic;
using Code.Gameplay.Interacting.Services;
using Entitas;

namespace Code.Gameplay.Produce.Anvil
{
    public class AnvilProduceSystem : IExecuteSystem
    {
        private readonly IGrabbableFactory _grabbableFactory;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(16);

        public AnvilProduceSystem(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProduceProgress, GameMatcher.Anvil, GameMatcher.AnvilQuality));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.produceProgress.Progress < 1f)
                    continue;

                GameEntity grabbableEntity = _grabbableFactory.SpawnAtPosition(entity.anvil.CurrentProduceItem, entity.transform.Value.position, false);
                grabbableEntity.AddQuality(entity.anvilQuality.Quality);
                entity.AddGrabbedItem(grabbableEntity.id.Value);
                entity.RemoveProduceProgress();
                entity.RemoveAnvilQuality();
            }
        }
    }
}