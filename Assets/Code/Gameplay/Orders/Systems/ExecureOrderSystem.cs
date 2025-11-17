using Code.Gameplay.Orders.Services;
using Entitas;

namespace Code.Gameplay.Orders.Systems
{
    public class ExecuteOrderSystem : IExecuteSystem
    {
        private readonly IOrderFactory _orderFactory;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _releasedItems;
        private readonly IGroup<GameEntity> _orders;

        public ExecuteOrderSystem(IOrderFactory orderFactory)
        {
            _orderFactory = orderFactory;
            
            _game = Contexts.sharedInstance.game;
            _releasedItems = _game.GetGroup(GameMatcher.AllOf(GameMatcher.GrabbableItem, GameMatcher.ReleasedAsOrder));
            _orders = _game.GetGroup(GameMatcher.Order);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _releasedItems)
            {
                if (!DoesExistCorrespondingOrder(entity, out GameEntity order))
                    return;
             
                order.isDestructed = true;
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
            return false;
        }
    }
}