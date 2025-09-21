using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Player
{
    [Game] public class Player : IComponent {}
    [Game] public class SpeedComponent : IComponent { public float Value; }
}
