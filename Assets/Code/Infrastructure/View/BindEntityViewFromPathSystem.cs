using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.View
{
    public class BindEntityViewFromPathSystem : IExecuteSystem
    {
        private readonly IEntityViewFactory _entityViewFactory;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(128);

        public BindEntityViewFromPathSystem(IEntityViewFactory entityViewFactory)
        {
            _entityViewFactory = entityViewFactory;

            GameContext game = Contexts.sharedInstance.game;

            _entities = game.GetGroup(GameMatcher.AllOf(GameMatcher.ViewPath).NoneOf(GameMatcher.View));
        }
        
        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                _entityViewFactory.CreateViewForEntityFromPath(entity);
            }
        }
    }
}