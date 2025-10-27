using System.Collections.Generic;
using Code.Gameplay.Grabbing;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Produce
{
    [Game, Unique] public class ProducingByPlayerComponent : IComponent { }
    [Game] public class ProduceProgress : IComponent { public float Progress; public ItemsEnum Item; }
    [Game] public class ProduceMachineComponent : IComponent { public ItemsEnum From; public ItemsEnum To; }
    [Game] public class WorkbenchComponent : IComponent { public List<int> Value; }
    [Game] public class QualityComponent : IComponent { public float Value; }
}
