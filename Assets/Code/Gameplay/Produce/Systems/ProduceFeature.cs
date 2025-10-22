using Code.Infrastructure.Systems;

namespace Code.Gameplay.Produce.Systems
{
    public class ProduceFeature : Feature
    {
        public ProduceFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProduceStatusIncreaser>());
            Add(systemFactory.Create<ItemProduceByMachineSystem>());
        }
    }
}