using Code.Gameplay.Player.View;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Player
{
    [Game, Unique] public class Player : IComponent {}
    [Game] public class GrabbedItem : IComponent { public int Value; }
    [Game] public class PlayerAnimatorComponent : IComponent { public PlayerAnimationController Value; }
    [Game] public class CurrentSpeedComponent : IComponent { public float Value; }
}