using Code.Gameplay.Produce;
using Entitas;

namespace Code.Gameplay.Grabbing
{
    [Game] public class GrabbableItem : IComponent { public ItemsEnum Value; }
    [Game] public class ShelfComponent : IComponent { }
    [Game] public class InfinityBoxComponent : IComponent { public ItemsEnum Value; }
    [Game] public class MoldComponent : IComponent { public MoldEnum Value;}
}


