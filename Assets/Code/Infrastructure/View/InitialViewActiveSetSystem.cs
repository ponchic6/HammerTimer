using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.View
{
    public class ViewActiveSetSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(64);

        public ViewActiveSetSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Transform, GameMatcher.ViewState, GameMatcher.View));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                entity.transform.Value.gameObject.SetActive(entity.viewState.Value);
                entity.RemoveViewState();
            }
        }
    }
}