using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Player
{
    [Game, Unique] public class Player : IComponent {}
    [Game] public class GrabbedItem : IComponent { public string Value; }
}