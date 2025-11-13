using Code.Gameplay.Produce.View;
using Entitas;

namespace Code.Gameplay.Orders
{
    [Game] public class Order : IComponent { public ItemsEnum Item; public float Timer; }
    [Game] public class OrderCreationCooldown : IComponent { public float Timer; }
}