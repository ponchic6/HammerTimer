using Code.Infrastructure.Systems;

namespace Code.Gameplay.Input.Systems
{
    public class InputFeature : Feature
    {
        public InputFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InitializeInputSystem>());
            Add(systemFactory.Create<MovementInputSystem>());
            Add(systemFactory.Create<GrabInputSystem>());
        }
    }
}