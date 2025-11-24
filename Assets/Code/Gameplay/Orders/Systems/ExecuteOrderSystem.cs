using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Orders.Systems
{
    public class ExecuteOrderSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _releasedItems;
        private readonly IGroup<GameEntity> _orders;
        private List<GameEntity> _buffer = new(8);

        public ExecuteOrderSystem()
        {
            _game = Contexts.sharedInstance.game;
            _releasedItems = _game.GetGroup(GameMatcher.AllOf(GameMatcher.GrabbableItem, GameMatcher.ReleasedAsOrder));
            _orders = _game.GetGroup(GameMatcher.Order);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _releasedItems.GetEntities(_buffer))
            {
                if (!DoesExistCorrespondingOrder(entity, out GameEntity order))
                    continue;
             
                order.AddSelfDestructTimer(1f);
                
                if (_game.playerEntity.hasGrabbedItem) 
                    _game.playerEntity.RemoveGrabbedItem();
                
                entity.isDestructed = true;
            }
        }

        private bool DoesExistCorrespondingOrder(GameEntity item, out GameEntity order)
        {
            foreach (GameEntity entity in _orders)
            {
                if (entity.order.Item == item.grabbableItem.Value)
                {
                    order = entity;
                    return true;
                }
            }
            
            order = null;
            item.isReleasedAsOrder = false;
            return false;
        }
    }
}