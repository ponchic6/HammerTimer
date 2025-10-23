using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Produce.StaticData
{
    [CreateAssetMenu(fileName = "WorkbenchRecipe", menuName = "StaticData/WorkbenchRecipe")]
    public class WorkbenchRecipe : ScriptableObject
    {
        public List<string> from;
        public string to;
    }
}