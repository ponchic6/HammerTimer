using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using UnityEngine;

namespace Code.Gameplay.Produce.StaticData
{
    [CreateAssetMenu(fileName = "WorkbenchRecipe", menuName = "StaticData/WorkbenchRecipe")]
    public class WorkbenchRecipe : ScriptableObject
    {
        public List<ItemsEnum> from;
        public ItemsEnum to;
    }
}