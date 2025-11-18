using System.Collections.Generic;
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
            List<ItemsEnum> list = new List<ItemsEnum>();
            list.Add(ItemsEnum.Axe);
            list.Add(ItemsEnum.Horseshoe);
            list.Add(ItemsEnum.Nail);
            list.Add(ItemsEnum.Sword);

            ItemsEnum randomItem = list[Random.Range(0, list.Count)];
            order.AddOrder(randomItem, _commonStaticData.orderTimer);
        }
    }
}