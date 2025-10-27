namespace Code.Infrastructure.View.Extensions
{
    public static class GameEntityExtension
    {
        public static void DisableView(this GameEntity entity)
        {
            entity.transform.Value.gameObject.SetActive(false);
        }
        
        public static void EnableView(this GameEntity entity)
        {
            entity.transform.Value.gameObject.SetActive(true);
        }
    }
}