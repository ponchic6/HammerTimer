using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Gameplay.Produce.View;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EntitasExtensionsEditor
{
    /// <summary>
    /// Автоматический валидатор для проверки, что все предметы из ItemsEnum имеют соответствующие префабы в папке GrabbableItems.
    /// </summary>
    public static class ItemsEnumValidator
    {
        private const string GrabbableItemsPath = "Assets/Resources/GrabbableItems";

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            ValidateItemsPrefabsExistence();
        }

        [MenuItem("Tools/Validate Items-Prefabs Correspondence")]
        public static void ValidateItemsPrefabsExistence()
        {
            List<string> missingPrefabs = new List<string>();
            List<string> extraPrefabs = new List<string>();

            // Получаем все значения из ItemsEnum
            ItemsEnum[] itemValues = (ItemsEnum[])Enum.GetValues(typeof(ItemsEnum));
            HashSet<string> itemNames = new HashSet<string>(itemValues.Select(i => i.ToString()));

            // Исключаем NoItem из проверки
            itemNames.Remove("NoItem");

            // Получаем все префабы ТОЛЬКО из папки GrabbableItems (без подпапок)
            string[] allPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { GrabbableItemsPath });
            HashSet<string> prefabNames = new HashSet<string>();

            foreach (string guid in allPrefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string directoryPath = Path.GetDirectoryName(path);

                // Проверяем, что префаб находится непосредственно в GrabbableItemsPath, а не в подпапках
                if (directoryPath.Replace("\\", "/") == GrabbableItemsPath)
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    prefabNames.Add(fileName);
                }
            }

            // Проверяем, что для каждого элемента ItemsEnum есть префаб
            foreach (string itemName in itemNames)
            {
                if (!prefabNames.Contains(itemName))
                {
                    missingPrefabs.Add(itemName);
                }
            }

            // Проверяем, что для каждого префаба есть элемент в ItemsEnum
            foreach (string prefabName in prefabNames)
            {
                if (!itemNames.Contains(prefabName))
                {
                    extraPrefabs.Add(prefabName);
                }
            }

            // Выводим результат только в лог
            if (missingPrefabs.Count > 0)
            {
                Debug.LogError($"ItemsEnum validation FAILED: Missing prefabs for items: {string.Join(", ", missingPrefabs)}");
            }

            if (extraPrefabs.Count > 0)
            {
                Debug.LogError($"ItemsEnum validation FAILED: Prefabs not in ItemsEnum: {string.Join(", ", extraPrefabs)}");
            }

            if (missingPrefabs.Count == 0 && extraPrefabs.Count == 0)
            {
                Debug.Log($"ItemsEnum validation PASSED: All {itemNames.Count} items have corresponding prefabs and all prefabs are in ItemsEnum");
            }
        }

        /// <summary>
        /// Проверяет, существует ли префаб для данного предмета
        /// </summary>
        public static bool PrefabExistsForItem(ItemsEnum item)
        {
            if (item == ItemsEnum.NoItem)
                return false;

            string itemName = item.ToString();
            string[] prefabGuids = AssetDatabase.FindAssets($"{itemName} t:Prefab", new[] { GrabbableItemsPath });

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);

                if (fileName == itemName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Получает путь к префабу для данного предмета
        /// </summary>
        public static string GetPrefabPathForItem(ItemsEnum item)
        {
            if (item == ItemsEnum.NoItem)
                return null;

            string itemName = item.ToString();
            string[] prefabGuids = AssetDatabase.FindAssets($"{itemName} t:Prefab", new[] { GrabbableItemsPath });

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);

                if (fileName == itemName)
                    return path;
            }

            return null;
        }

        /// <summary>
        /// Возвращает словарь всех предметов и путей к их префабам
        /// </summary>
        public static Dictionary<ItemsEnum, string> GetItemPrefabMapping()
        {
            Dictionary<ItemsEnum, string> mapping = new Dictionary<ItemsEnum, string>();

            ItemsEnum[] itemValues = (ItemsEnum[])Enum.GetValues(typeof(ItemsEnum));

            foreach (ItemsEnum item in itemValues)
            {
                if (item == ItemsEnum.NoItem)
                    continue;

                string path = GetPrefabPathForItem(item);
                if (!string.IsNullOrEmpty(path))
                {
                    mapping[item] = path;
                }
            }

            return mapping;
        }
    }
}
