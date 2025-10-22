using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input
{
    [Game] public class Input : IComponent {}
    [Game] public class InteractInput : IComponent {}
    [Game] public class MovementInputComponent : IComponent { public Vector2 Value; }
}