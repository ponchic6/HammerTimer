using Code.Gameplay.Input;
using Code.Gameplay.Input.Systems;
using Code.Gameplay.Player.Systems;
using Code.Gameplay.Produce.Systems;
using Code.Infrastructure.Destroy;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;

namespace Code.Gameplay
{
    public class MainFeature : Feature
    {
        public MainFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ViewFeature>());
            Add(systemFactory.Create<InputFeature>());
            Add(systemFactory.Create<PlayerFeature>());
            Add(systemFactory.Create<ProduceFeature>());
            Add(systemFactory.Create<DestroyFeature>());
        }
    }
}