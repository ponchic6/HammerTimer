namespace Code.Infrastructure.Services
{
    public class IdentifierService : IIdentifierService
    {
        private int _id;

        public int Next()
        {
            _id++;
            return _id;
        }
    }
}