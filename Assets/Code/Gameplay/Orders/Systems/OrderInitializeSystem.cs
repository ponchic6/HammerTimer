using Code.Infrastructure.Services;
using Code.Infrastructure.StaticData;
using Entitas;

namespace Code.Gameplay.Orders.Systems
{
    public class OrderInitializeSystem : IInitializeSystem
    {
        private readonly IIdentifierService _identifierService;
        private readonly CommonStaticData _commonStaticData;
        private readonly GameContext _game;

        public OrderInitializeSystem(IIdentifierService identifierService, CommonStaticData commonStaticData)
        {
            _identifierService = identifierService;
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;
        }
        
        public void Initialize()
        {
            GameEntity orderCooldown = _game.CreateEntity();
            orderCooldown.AddId(_identifierService.Next());
            orderCooldown.AddOrderCreationCooldown(_commonStaticData.orderCreationCooldown);
        }
    }
}