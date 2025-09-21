using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.Destroy.Systems
{
    public class CleanupGameDestructedSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private List<GameEntity> _buffer = new(64);

        public CleanupGameDestructedSystem()
        {
            var game = Contexts.sharedInstance.game;

            _entities = game.GetGroup(GameMatcher.Destructed);
        }
        
        public void Cleanup()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                entity.Destroy();
            }
        }
    }
}