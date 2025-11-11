using Code.Gameplay.Produce.Anvil;
using Code.Gameplay.Produce.Forge;
using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.ProduceMachine;
using Code.Gameplay.Produce.Workbench;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Produce.Systems
{
    public class ProduceFeature : Feature
    {
        public ProduceFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProduceMachineProduceSystem>());
            
            Add(systemFactory.Create<ProgressAndQualityForgingSystem>());
            Add(systemFactory.Create<CoolingOnAnvilSystem>());
            Add(systemFactory.Create<AnvilProduceSystem>());
            
            Add(systemFactory.Create<WorkbenchRecipeValidateSystem>());
            Add(systemFactory.Create<WorkbenchProgressIncreaseSystem>());
            Add(systemFactory.Create<WorkbenchProduceSystem>());
            
            Add(systemFactory.Create<ForgeCoalBurnSystem>());
            Add(systemFactory.Create<TemperatureIncreaseByForgeSystem>());
            Add(systemFactory.Create<CoolingOutsideForgeSystem>());
            Add(systemFactory.Create<IronAggregationStateSystem>());
            Add(systemFactory.Create<IronTemperatureAddSystem>());

            Add(systemFactory.Create<MouldingCalculateQualitySystem>());
            Add(systemFactory.Create<AddMoldToClayFormSystem>());
        }
    }
}