using Code.Infrastructure.Systems;

namespace Code.Infrastructure.View
{
    public class ViewFeature : Feature
    {
        public ViewFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<BindEntityViewFromPathSystem>());
            Add(systemFactory.Create<BindEntityViewFromPrefabSystem>());
            Add(systemFactory.Create<BindEntityViewFromPrefabWithParentSystem>());
            Add(systemFactory.Create<InitialPositionSetSystem>());
        }
    }
}