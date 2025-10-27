using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Code.Gameplay.Grabbing;
using Code.Infrastructure.StaticData;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ItemsEnumGenerator : EditorWindow
    {
        private const string GrabbableItemsPath = "Assets/Resources/GrabbableItems";
        private const string EnumFilePath = "Assets/Code/Gameplay/Grabbing/ItemsEnum.cs";
        private const string CommonStaticDataPath = "Assets/Resources/CommonStaticData.asset";

        [MenuItem("Tools/Generate Items Enum")]
        public static void ShowWindow()
        {
            GetWindow<ItemsEnumGenerator>("Items Enum Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Items Enum Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Эта утилита сгенерирует ItemsEnum на основе префабов в папке GrabbableItems\n" +
                "и обновит enumPathPairsList в CommonStaticData.",
                MessageType.Info);

            GUILayout.Space(10);

            if (GUILayout.Button("Сгенерировать ItemsEnum", GUILayout.Height(40)))
            {
                GenerateItemsEnum();
            }
        }

        private static void GenerateItemsEnum()
        {
            // Получаем все префабы из папки GrabbableItems
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { GrabbableItemsPath });
            
            if (prefabGuids.Length == 0)
            {
                EditorUtility.DisplayDialog("Ошибка", 
                    $"Не найдено префабов в папке {GrabbableItemsPath}", "OK");
                return;
            }

            List<string> itemNames = new List<string>();
            Dictionary<string, string> itemPaths = new Dictionary<string, string>();

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                itemNames.Add(fileName);
                
                // Сохраняем путь относительно Resources
                string resourcePath = path.Replace("Assets/Resources/", "").Replace(".prefab", "");
                itemPaths[fileName] = resourcePath;
            }

            // Сортируем для консистентности
            itemNames.Sort();

            // Генерируем код enum
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace Code.Gameplay.Grabbing");
            sb.AppendLine("{");
            sb.AppendLine("    public enum ItemsEnum");
            sb.AppendLine("    {");

            for (int i = 0; i < itemNames.Count; i++)
            {
                string itemName = itemNames[i];
                sb.Append($"        {itemName}");
                if (i < itemNames.Count - 1)
                {
                    sb.AppendLine(",");
                }
                else
                {
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Записываем файл
            File.WriteAllText(EnumFilePath, sb.ToString());
            AssetDatabase.Refresh();

            Debug.Log($"ItemsEnum успешно сгенерирован с {itemNames.Count} элементами");

            // Обновляем CommonStaticData
            UpdateCommonStaticData(itemNames, itemPaths);
        }

        private static void UpdateCommonStaticData(List<string> itemNames, Dictionary<string, string> itemPaths)
        {
            CommonStaticData commonStaticData = AssetDatabase.LoadAssetAtPath<CommonStaticData>(CommonStaticDataPath);
            
            if (commonStaticData == null)
            {
                EditorUtility.DisplayDialog("Ошибка", 
                    $"Не найден CommonStaticData по пути {CommonStaticDataPath}", "OK");
                return;
            }

            // Очищаем старый список
            if (commonStaticData.enumPathPairsList == null)
            {
                commonStaticData.enumPathPairsList = new List<EnumPathPair>();
            }
            else
            {
                commonStaticData.enumPathPairsList.Clear();
            }

            // Ждем компиляции для обновления enum
            EditorUtility.DisplayProgressBar("Обновление CommonStaticData", 
                "Ожидание компиляции...", 0.5f);

            // Используем EditorApplication.delayCall для выполнения после компиляции
            EditorApplication.delayCall += () =>
            {
                // Перезагружаем asset после компиляции
                commonStaticData = AssetDatabase.LoadAssetAtPath<CommonStaticData>(CommonStaticDataPath);
                
                if (commonStaticData.enumPathPairsList == null)
                {
                    commonStaticData.enumPathPairsList = new List<EnumPathPair>();
                }

                // Добавляем новые пары
                foreach (string itemName in itemNames)
                {
                    if (System.Enum.TryParse<ItemsEnum>(itemName, out ItemsEnum enumValue))
                    {
                        commonStaticData.enumPathPairsList.Add(new EnumPathPair
                        {
                            @enum = enumValue,
                            path = itemPaths[itemName]
                        });
                    }
                }

                EditorUtility.SetDirty(commonStaticData);
                AssetDatabase.SaveAssets();
                EditorUtility.ClearProgressBar();

                Debug.Log($"CommonStaticData обновлен: добавлено {commonStaticData.enumPathPairsList.Count} пар enum-путь");
                
                EditorUtility.DisplayDialog("Успех", 
                    $"ItemsEnum и CommonStaticData успешно обновлены!\n" +
                    $"Элементов enum: {itemNames.Count}\n" +
                    $"Пар enum-путь: {commonStaticData.enumPathPairsList.Count}", "OK");
            };
        }
    }
}
