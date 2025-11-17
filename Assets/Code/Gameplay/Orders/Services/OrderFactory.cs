using Code.Gameplay.Produce.View;
using Code.Infrastructure.Services;
using Code.Infrastructure.StaticData;
using UnityEngine;

namespace Code.Gameplay.Orders.Services
{
    public class OrderFactory : IOrderFactory
    {
        private readonly IIdentifierService _identifierService;
        private readonly CommonStaticData _commonStaticData;
        private readonly GameContext _game;
        private GameObject _orderCanvas;

        public OrderFactory(IIdentifierService identifierService, CommonStaticData commonStaticData)
        {
            _identifierService = identifierService;
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;
        }
        
        public void CreateRandomOrder()
        {
            GameEntity order = _game.CreateEntity();
            order.AddId(_identifierService.Next());
            order.AddOrder(ItemsEnum.Axe, _commonStaticData.orderTimer);
        }
    }
}