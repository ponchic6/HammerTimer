using Code.Infrastructure.Destroy.Systems;
using Code.Infrastructure.Systems;

namespace Code.Infrastructure.Destroy
{
    public class DestroyFeature : Feature
    {
        public DestroyFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<SelfDestructTimerSystem>());
            
            Add(systemFactory.Create<CleanupGameDestructedViewSystem>());
            Add(systemFactory.Create<CleanupGameDestructedSystem>());
        }
    }
}