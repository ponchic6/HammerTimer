using Code.Gameplay.Orders.Services;
using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Orders.Systems
{
    public class OrderCreationSystem : IExecuteSystem
    {
        private readonly IOrderFactory _orderFactory;
        private readonly CommonStaticData _commonStaticData;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _orders;

        public OrderCreationSystem(IOrderFactory orderFactory, CommonStaticData commonStaticData)
        {
            _orderFactory = orderFactory;
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.OrderCreationCooldown);
            _orders = _game.GetGroup(GameMatcher.Order);
        }
        
        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                entity.orderCreationCooldown.Timer -= Time.deltaTime;

                if (entity.orderCreationCooldown.Timer <= 0)
                {
                    entity.orderCreationCooldown.Timer = _commonStaticData.orderCreationCooldown;

                    if (_orders.count < _commonStaticData.maxOrdersCount)
                        _orderFactory.CreateRandomOrder();
                    
                    continue;
                }

                if (_orders.count == 0)
                {
                    _orderFactory.CreateRandomOrder();
                }
            }
        }
    }
}