using Entitas;

namespace Code.Infrastructure.Systems
{
    public interface ISystemFactory
    {
        T Create<T>() where T : ISystem;
    }
}
