using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.View
{
    public class InitialPositionSetSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(256);

        public InitialPositionSetSystem()
        {
            GameContext game = Contexts.sharedInstance.game;
            _entities = game.GetGroup(GameMatcher.AllOf(GameMatcher.Transform, GameMatcher.InitialTransform));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                entity.transform.Value.position = entity.initialTransform.Position;
                entity.transform.Value.rotation = entity.initialTransform.Rotation;
                entity.RemoveInitialTransform();
            }
        }
    }
}