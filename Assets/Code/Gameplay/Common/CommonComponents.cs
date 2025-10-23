using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Gameplay.Common
{
    [Game] public class Id : IComponent { [PrimaryEntityIndex] public int Value; }
    [Game] public class TransformComponent : IComponent { public Transform Value; }
    [Game] public class SpeedComponent : IComponent { public float Value; }
}