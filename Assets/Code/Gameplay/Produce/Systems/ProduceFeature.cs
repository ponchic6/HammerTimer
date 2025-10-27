using Code.Infrastructure.Systems;

namespace Code.Gameplay.Produce.Systems
{
    public class ProduceFeature : Feature
    {
        public ProduceFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProduceIncreaseByPlayerSystem>());
            Add(systemFactory.Create<ProduceMachineProduceSystem>());
            
            Add(systemFactory.Create<WorkbenchRecipeValidateSystem>());
            Add(systemFactory.Create<WorkbenchProduceSystem>());
            
            Add(systemFactory.Create<ForgeCoalBurnSystem>());
            Add(systemFactory.Create<TemperatureIncreaseByForgeSystem>());
            Add(systemFactory.Create<CoolingOutsideForgeSystem>());
            Add(systemFactory.Create<IronAggregationStateSystem>());
            Add(systemFactory.Create<IronTemperatureAddSystem>());

            Add(systemFactory.Create<MouldingCalculateQualitySystem>());
        }
    }
}