namespace Code.Infrastructure.View
{
    public interface IEntityViewFactory
    {
        public EntityBehaviour CreateViewForEntityFromPath(GameEntity gameEntity);
        public EntityBehaviour CreateViewForEntityFromPrefab(GameEntity gameEntity);
    }
}