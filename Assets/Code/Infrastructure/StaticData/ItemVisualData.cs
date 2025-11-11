using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using UnityEngine;

namespace Code.Infrastructure.StaticData
{
    [CreateAssetMenu(fileName = "ItemVisualData", menuName = "StaticData/ItemVisualData")]
    public class ItemVisualData : ScriptableObject
    {
        public List<EnumVisualPair> prefabs;
    }
}