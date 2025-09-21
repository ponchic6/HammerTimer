using Entitas;

namespace Code.Infrastructure.Destroy
{
    [Game] public class Destructed : IComponent { }
    [Game] public class SelfDestructTimer : IComponent { public float Value; }
}