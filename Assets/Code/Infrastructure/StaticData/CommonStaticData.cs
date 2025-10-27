using System;
using System.Collections.Generic;
using Code.Gameplay.Grabbing;
using Code.Gameplay.Produce.StaticData;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Infrastructure.StaticData
{
    [CreateAssetMenu(fileName = "CommonStaticData", menuName = "StaticData/CommonStaticData")]
    public class CommonStaticData : ScriptableObject
    {
        public EntityBehaviour playerPrefab;
        public EntityBehaviour shelf;
        public EntityBehaviour produceMachine;
        public float doubleClickThreshold = 0.2f;
        public AnimationCurve qualityTimeCurve;
        public List<WorkbenchRecipe> workbenchRecipes;
        public List<EnumPathPair> enumPathPairsList;
        public float forgeTemperatureIncreaseRate;
        public float forgeTemperatureDecreaseRate;
        public float forgeMaxTemperature;
        public float meltingTemperature;
        public float itemTemperatureChangeRate;
        public float itemCoolingRate;
        public float environmentTemperature;

        public Dictionary<ItemsEnum, string> EnumPathPairs = new();

        private void OnEnable()
        {
            EnumPathPairs.Clear();
            foreach (EnumPathPair pair in enumPathPairsList)
            {
                if (!EnumPathPairs.ContainsKey(pair.@enum))
                {
                    EnumPathPairs.Add(pair.@enum, pair.path);
                }
            }
        }
    }

    [Serializable]
    public class EnumPathPair
    {
        public ItemsEnum @enum;
        public string path;
    }
}

