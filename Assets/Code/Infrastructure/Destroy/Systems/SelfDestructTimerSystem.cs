using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Infrastructure.Destroy.Systems
{
    public class SelfDestructTimerSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new (128);

        public SelfDestructTimerSystem()
        {
            GameContext game = Contexts.sharedInstance.game;
            _entities = game.GetGroup(GameMatcher.SelfDestructTimer);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                if (entity.selfDestructTimer.Value > 0) 
                    entity.ReplaceSelfDestructTimer(entity.selfDestructTimer.Value - Time.deltaTime);
                else
                {
                    entity.RemoveSelfDestructTimer();
                    entity.isDestructed = true;
                }
            }
        }
    }
}