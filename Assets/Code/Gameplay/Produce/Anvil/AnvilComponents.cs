using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using Entitas;

namespace Code.Gameplay.Produce.Anvil
{ 
    [Game] public class Anvil : IComponent { public ItemsEnum CurrentProduceItem; public List<ItemsEnum> PossibleItems; };
    [Game] public class AnvilQualityComponent : IComponent { public float Quality; public float Temperature; };
}