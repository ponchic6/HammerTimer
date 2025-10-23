using Code.Infrastructure.Systems;

namespace Code.Gameplay.Produce.Systems
{
    public class ProduceFeature : Feature
    {
        public ProduceFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<WorkbenchRecipeValidateSystem>());
            Add(systemFactory.Create<ForgeCoalBurnSystem>());
            Add(systemFactory.Create<ForgeMeltingSystem>());
            Add(systemFactory.Create<ProduceStatusIncreaser>());
            Add(systemFactory.Create<ItemProduceSystem>());
            Add(systemFactory.Create<WorkbenchProduceSystem>());
        }
    }
}