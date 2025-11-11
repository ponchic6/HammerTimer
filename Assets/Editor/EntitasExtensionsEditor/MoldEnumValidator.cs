using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.View;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EntitasExtensionsEditor
{
    /// <summary>
    /// Автоматический валидатор для проверки, что все формы из MoldEnum присутствуют в ItemsEnum.
    /// </summary>
    public static class MoldEnumValidator
    {
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            ValidateMoldEnumCorrespondence();
        }

        [MenuItem("Tools/Validate Mold-Item Correspondence")]
        public static void ValidateMoldEnumCorrespondence()
        {
            List<string> missingMolds = new List<string>();

            // Получаем все значения из MoldEnum и ItemsEnum
            MoldEnum[] moldValues = (MoldEnum[])Enum.GetValues(typeof(MoldEnum));
            ItemsEnum[] itemValues = (ItemsEnum[])Enum.GetValues(typeof(ItemsEnum));

            HashSet<string> itemNames = new HashSet<string>(itemValues.Select(i => i.ToString()));

            // Проверяем, что каждая форма есть в ItemsEnum
            foreach (MoldEnum mold in moldValues)
            {
                string moldName = mold.ToString();

                // Пропускаем NoMold
                if (moldName == "NoMold")
                    continue;

                // Проверяем наличие формы в ItemsEnum
                if (!itemNames.Contains(moldName))
                {
                    missingMolds.Add(moldName);
                }
            }

            // Выводим результат
            if (missingMolds.Count > 0)
            {
                string errorMessage = $"MoldEnum validation FAILED: Missing {missingMolds.Count} mold(s) in ItemsEnum: {string.Join(", ", missingMolds)}";
                Debug.LogError(errorMessage);

                EditorUtility.DisplayDialog(
                    "MoldEnum Validation Failed",
                    errorMessage,
                    "OK"
                );
            }
            else
            {
                Debug.Log("MoldEnum validation PASSED: All molds are present in ItemsEnum");
            }
        }

        /// <summary>
        /// Проверяет, есть ли соответствующий предмет для данной формы
        /// </summary>
        public static bool TryGetItemFromMold(MoldEnum mold, out ItemsEnum item)
        {
            string moldName = mold.ToString();

            if (moldName == "NoMold")
            {
                item = ItemsEnum.NoItem;
                return false;
            }

            if (!moldName.EndsWith("Mold"))
            {
                item = ItemsEnum.NoItem;
                return false;
            }

            string itemName = moldName.Replace("Mold", "");

            if (Enum.TryParse<ItemsEnum>(itemName, out item))
            {
                return true;
            }

            item = ItemsEnum.NoItem;
            return false;
        }

        /// <summary>
        /// Возвращает словарь соответствий MoldEnum → ItemsEnum
        /// </summary>
        public static Dictionary<MoldEnum, ItemsEnum> GetMoldItemMapping()
        {
            Dictionary<MoldEnum, ItemsEnum> mapping = new Dictionary<MoldEnum, ItemsEnum>();

            MoldEnum[] moldValues = (MoldEnum[])Enum.GetValues(typeof(MoldEnum));

            foreach (MoldEnum mold in moldValues)
            {
                if (TryGetItemFromMold(mold, out ItemsEnum item))
                {
                    mapping[mold] = item;
                }
            }

            return mapping;
        }
    }
}
