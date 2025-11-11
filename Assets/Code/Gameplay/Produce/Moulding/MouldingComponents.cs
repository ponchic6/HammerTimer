using Code.Gameplay.Produce.View;
using Entitas;

namespace Code.Gameplay.Produce.Moulding
{
    [Game] public class MouldingMachineComponent : IComponent { public MoldEnum MoldEnum; public ItemsEnum Item;}
    [Game] public class MouldingQualityComponent : IComponent { public float Quality; public float StartTime; }
}