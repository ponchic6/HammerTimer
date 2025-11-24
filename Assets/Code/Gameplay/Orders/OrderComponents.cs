using Code.Gameplay.Produce.View;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Orders
{
    [Game] public class Order : IComponent { public ItemsEnum Item; public float Timer; }
    [Game] public class OrderCreationCooldown : IComponent { public float Timer; }
    [Game] public class OrderReleaseZone : IComponent { }
    [Unique][Game] public class ReleasedAsOrder : IComponent { }
}