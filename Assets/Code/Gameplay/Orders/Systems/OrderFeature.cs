using Code.Infrastructure.Systems;

namespace Code.Gameplay.Orders.Systems
{
    public class OrderFeature : Feature
    {
        public OrderFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<OrderInitializeSystem>());
            Add(systemFactory.Create<OrderCreationSystem>());
            Add(systemFactory.Create<OrderTimerSystem>());
            Add(systemFactory.Create<ExecuteOrderSystem>());
        }
    }
}