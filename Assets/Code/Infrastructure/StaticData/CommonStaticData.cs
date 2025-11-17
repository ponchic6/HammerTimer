using System;
using System.Collections.Generic;
using Code.Gameplay.Produce.StaticData;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Infrastructure.StaticData
{
    [CreateAssetMenu(fileName = "CommonStaticData", menuName = "StaticData/CommonStaticData")]
    public class CommonStaticData : ScriptableObject
    {
        public ItemSpritesData itemSpritesData;
        public ItemVisualData itemVisualData;
        public EntityBehaviour playerPrefab;
        public EntityBehaviour shelf;
        public EntityBehaviour produceMachine;
        public float doubleClickThreshold;
        public AnimationCurve qualityTimeCurve;
        public float moldingDuration;
        public List<WorkbenchRecipe> workbenchRecipes;
        public float maxPlayerSpeed;
        public float acceleration;
        public float forgePowerDecreaseBase;
        public float oneCoalPower;
        public float forgeMaxPower;
        public float meltingTemperature;
        public float itemTemperatureChangeRate;
        public float itemCoolingRate;
        public float environmentTemperature;
        public float temperatureForMaxForgingQuality;
        public float orderCreationCooldown;
        public float orderTimer;
        public int maxOrdersCount;

        public Dictionary<ItemsEnum, GameObject> EnumVisualPairs = new();

        private void OnEnable()
        {
            EnumVisualPairs.Clear();
            foreach (EnumVisualPair pair in itemVisualData.prefabs)
            {
                if (!EnumVisualPairs.ContainsKey(pair.@enum))
                {
                    EnumVisualPairs.Add(pair.@enum, pair.visual);
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

