using Code.Infrastructure.Systems;

namespace Code.Gameplay.Orders.Systems
{
    public class OrderFeature : Feature
    {
        public OrderFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<OrderCreationSystem>());
            Add(systemFactory.Create<OrderTimerSystem>());
        }
    }
}