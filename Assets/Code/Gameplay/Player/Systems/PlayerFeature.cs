using Code.Infrastructure.Systems;

namespace Code.Gameplay.Player.Systems
{
    public class PlayerFeature : Feature
    {
        public PlayerFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InitializePlayerSystem>());
            Add(systemFactory.Create<PlayerMovementSystem>());
        }
    }
}
