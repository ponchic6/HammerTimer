using Code.Gameplay.Grabbing;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Player
{
    [Game][Unique] public class Player : IComponent {}
    [Game] public class SpeedComponent : IComponent { public float Value; }
    [Game] public class GrabbedItem : IComponent { public GrabbableEnum Value; }
}