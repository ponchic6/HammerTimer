using Code.Gameplay.Orders.Services;
using Entitas;

namespace Code.Gameplay.Orders.Systems
{
    public class OrderCreationSystem : IExecuteSystem
    {
        private readonly IOrderFactory _orderFactory;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public OrderCreationSystem(IOrderFactory orderFactory)
        {
            _orderFactory = orderFactory;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.Order);
        }
        
        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                
            }
        }
    }
}