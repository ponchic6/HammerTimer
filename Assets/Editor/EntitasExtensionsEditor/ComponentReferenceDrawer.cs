using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.View.Extensions;
using UnityEditor;
using UnityEngine;

namespace EntitasExtensionsEditor
{
    [CustomPropertyDrawer(typeof(ComponentReference))]
    public class ComponentReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty componentIndexProp = property.FindPropertyRelative("componentIndex");
            int currentIndex = componentIndexProp.intValue;

            HashSet<int> usedIndices = GetUsedComponentIndices(property);

            // Create a sorted list of component indices based on component names
            var sortedComponents = Enumerable.Range(0, GameComponentsLookup.TotalComponents)
                .Select(i => new { Index = i, Name = GameComponentsLookup.componentNames[i] })
                .OrderBy(c => c.Name)
                .ToList();

            string[] options = new string[GameComponentsLookup.TotalComponents + 1];
            int[] indexMap = new int[GameComponentsLookup.TotalComponents + 1];

            options[0] = "None";
            indexMap[0] = -1;

            for (int i = 0; i < sortedComponents.Count; i++)
            {
                int componentIndex = sortedComponents[i].Index;
                indexMap[i + 1] = componentIndex;

                if (usedIndices.Contains(componentIndex) && componentIndex != currentIndex)
                {
                    options[i + 1] = $"{componentIndex}: {sortedComponents[i].Name} (Already added)";
                }
                else
                {
                    options[i + 1] = $"{componentIndex}: {sortedComponents[i].Name}";
                }
            }

            // Find the dropdown index for the current component index
            int selectedDropdownIndex = 0;
            for (int i = 0; i < indexMap.Length; i++)
            {
                if (indexMap[i] == currentIndex)
                {
                    selectedDropdownIndex = i;
                    break;
                }
            }

            int newDropdownIndex = EditorGUI.Popup(position, label.text, selectedDropdownIndex, options);
            int newComponentIndex = indexMap[newDropdownIndex];

            if (newComponentIndex >= 0 && usedIndices.Contains(newComponentIndex) && newComponentIndex != currentIndex)
            {
                EditorUtility.DisplayDialog("Duplicate Component",
                    $"Component '{GameComponentsLookup.componentNames[newComponentIndex]}' is already added to the list!",
                    "OK");
            }
            else
            {
                componentIndexProp.intValue = newComponentIndex;
            }

            EditorGUI.EndProperty();
        }

        private HashSet<int> GetUsedComponentIndices(SerializedProperty currentProperty)
        {
            HashSet<int> usedIndices = new HashSet<int>();

            SerializedProperty listProperty = currentProperty.GetParentProperty();
            if (listProperty != null && listProperty.isArray)
            {
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                    SerializedProperty indexProp = element.FindPropertyRelative("componentIndex");
                    if (indexProp != null && indexProp.intValue >= 0)
                    {
                        usedIndices.Add(indexProp.intValue);
                    }
                }
            }

            return usedIndices;
        }
    }

    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty GetParentProperty(this SerializedProperty property)
        {
            string path = property.propertyPath;
            int lastDotIndex = path.LastIndexOf('.');

            if (lastDotIndex == -1)
                return null;

            string parentPath = path.Substring(0, lastDotIndex);

            if (parentPath.EndsWith("]"))
            {
                int arrayStartIndex = parentPath.LastIndexOf(".Array.data[");
                if (arrayStartIndex != -1)
                {
                    parentPath = parentPath.Substring(0, arrayStartIndex);
                }
            }

            return property.serializedObject.FindProperty(parentPath);
        }
    }
}
