using Code.Infrastructure.Services;
using Entitas;

namespace Code.Gameplay.Orders.Systems
{
    public class OrderInitializeSystem : IInitializeSystem
    {
        private readonly IIdentifierService _identifierService;
        private readonly GameContext _game;

        public OrderInitializeSystem(IIdentifierService identifierService)
        {
            _identifierService = identifierService;
            _game = Contexts.sharedInstance.game;
        }
        
        public void Initialize()
        {
            GameEntity orderCooldown = _game.CreateEntity();
            orderCooldown.AddId(_identifierService.Next());
            orderCooldown.AddOrderCreationCooldown(0f);
        }
    }
}