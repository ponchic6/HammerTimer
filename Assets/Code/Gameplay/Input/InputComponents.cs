using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Gameplay.Input
{
    [Game, Unique] public class Input : IComponent {}
    [Game] public class InteractDownInput : IComponent { }
    [Game] public class DoubleInteractDownInput : IComponent { }
    [Game] public class TimeOfInteractDownInput : IComponent { public TimeSpan TimeAdded; }
    [Game] public class HoldingInteractInput : IComponent { }
    [Game] public class MovementInputComponent : IComponent { public Vector2 Value; }
}