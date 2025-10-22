using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Grabbing
{
    [Game] public class GrabbableItem : IComponent { public GrabbableEnum Value; }
    [Game] public class SocketComponent : IComponent { }
    [Game] public class ProduceMachineComponent : IComponent { public GrabbableEnum From; public GrabbableEnum To; }
    [Game] public class ProduceProgress : IComponent { public float Value; }
    [Game, Unique] public class ProducingByPlayerComponent : IComponent { }
}


