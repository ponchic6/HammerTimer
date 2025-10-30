using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.View;
using Entitas;

namespace Code.Gameplay.Produce
{
    [Game] public class GrabbableItem : IComponent { public ItemsEnum Value; }
    [Game] public class ShelfComponent : IComponent { }
    [Game] public class InfinityBoxComponent : IComponent { public ItemsEnum Value; }
    [Game] public class MoldComponent : IComponent { public MoldEnum Mold; public ItemsEnum Item; }
}

