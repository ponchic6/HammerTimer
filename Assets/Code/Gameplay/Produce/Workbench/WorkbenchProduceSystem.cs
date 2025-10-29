using System.Collections.Generic;
using Code.Gameplay.Interacting.Services;
using Entitas;

namespace Code.Gameplay.Produce.Workbench
{
    public class WorkbenchProduceSystem : IExecuteSystem
    {
        private readonly IGrabbableFactory _grabbableFactory;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(4);

        public WorkbenchProduceSystem(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.ProduceProgress, GameMatcher.Workbench));
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

                DestructWorkbenchItems(entity);
            }
        }

        private void DestructWorkbenchItems(GameEntity entity)
        {
            foreach (int ingredientEntityId in entity.workbench.Value)
            {
                GameEntity ingredientEntity = _game.GetEntityWithId(ingredientEntityId);
                ingredientEntity.isDestructed = true;
            }

            entity.workbench.Value.Clear();
        }
    }
}