using Entitas;
using UnityEngine;

namespace Code.Infrastructure.Destroy.Systems
{
    public class CleanupGameDestructedViewSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public CleanupGameDestructedViewSystem()
        {
            var game = Contexts.sharedInstance.game;

            _entities = game.GetGroup(GameMatcher.AllOf(GameMatcher.View, GameMatcher.Destructed));
        }
        
        public void Cleanup()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.view.Value.ReleaseEntity();
                Object.Destroy(entity.view.Value.gameObject);
            }
        }
    }
}