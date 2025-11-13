using Entitas;
using UnityEngine;

namespace Code.Gameplay.Orders.Systems
{
    public class OrderTimerSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public OrderTimerSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.Order);
        }
        
        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.order.Timer -= Time.deltaTime;
                
                if (entity.order.Timer <= 0)
                    entity.isDestructed  = true;
            }
        }
    }
}