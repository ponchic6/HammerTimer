using Entitas;

namespace Code.Gameplay.Produce.Forge
{
    [Game] public class ForgeComponent : IComponent { public float Power;}
    [Game] public class GrabbableTemperatureComponent : IComponent { public float Value; }
}