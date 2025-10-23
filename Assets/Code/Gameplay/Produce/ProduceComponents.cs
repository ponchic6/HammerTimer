using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Produce
{
    [Game] public class ProduceMachineComponent : IComponent { public string From; public string To; }
    [Game] public class WorkbenchComponent : IComponent { public List<string> Value; }
    [Game] public class ProduceProgress : IComponent { public float Progress; public string Item; }
    [Game] public class ForgeComponent : IComponent { public float Coal; public float Oxygen; }
    [Game, Unique] public class ProducingByPlayerComponent : IComponent { }
}
