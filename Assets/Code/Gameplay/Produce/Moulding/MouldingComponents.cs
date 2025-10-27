using System;
using Entitas;

namespace Code.Gameplay.Produce.Moulding
{
    [Game] public class MouldingMachineComponent : IComponent { public MoldEnum MoldEnumValue; }
    [Game] public class MouldingQualityComponent : IComponent { public float Quality; public TimeSpan StartTime;}
}