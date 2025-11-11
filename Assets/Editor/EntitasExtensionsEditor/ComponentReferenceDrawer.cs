using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Infrastructure.View.Extensions;
using UnityEditor;
using UnityEngine;

namespace EntitasExtensionsEditor
{
    [CustomPropertyDrawer(typeof(ComponentReference))]
    public class ComponentReferenceDrawer : PropertyDrawer
    {
        private const float FieldHeight = 20f;
        private const float Spacing = 2f;
        private const float ListElementHeight = 18f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty componentIndexProp = property.FindPropertyRelative("componentIndex");
            int currentIndex = componentIndexProp.intValue;
            int previousIndex = currentIndex;

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

            Rect dropdownRect = new Rect(position.x, position.y, position.width, FieldHeight);
            int newDropdownIndex = EditorGUI.Popup(dropdownRect, label.text, selectedDropdownIndex, options);
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

                // If component changed, update field values
                if (newComponentIndex != previousIndex)
                {
                    UpdateFieldValues(property, newComponentIndex);
                }
            }

            // Draw component fields if a component is selected
            if (currentIndex >= 0 && currentIndex < GameComponentsLookup.TotalComponents)
            {
                float yOffset = FieldHeight + Spacing;
                DrawComponentFields(property, currentIndex, position, ref yOffset);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = FieldHeight; // Dropdown height

            SerializedProperty componentIndexProp = property.FindPropertyRelative("componentIndex");
            int currentIndex = componentIndexProp.intValue;

            if (currentIndex >= 0 && currentIndex < GameComponentsLookup.TotalComponents)
            {
                SerializedProperty fieldValuesProp = property.FindPropertyRelative("fieldValues");

                for (int i = 0; i < fieldValuesProp.arraySize; i++)
                {
                    SerializedProperty fieldData = fieldValuesProp.GetArrayElementAtIndex(i);
                    string fieldType = fieldData.FindPropertyRelative("fieldType").stringValue;

                    if (fieldType.StartsWith("System.Collections.Generic.List`1[[System.Int32"))
                    {
                        // List<int> header + elements + add button
                        SerializedProperty listProp = fieldData.FindPropertyRelative("intListValue");
                        height += FieldHeight; // Header
                        height += (ListElementHeight + Spacing) * listProp.arraySize; // Elements
                        height += FieldHeight + Spacing; // Add button
                    }
                    else if (fieldType.StartsWith("System.Collections.Generic.List`1[["))
                    {
                        // List<Enum> header + elements + add button
                        SerializedProperty listProp = fieldData.FindPropertyRelative("enumListValue");
                        height += FieldHeight; // Header
                        height += (ListElementHeight + Spacing) * listProp.arraySize; // Elements
                        height += FieldHeight + Spacing; // Add button
                    }
                    else
                    {
                        height += FieldHeight + Spacing;
                    }
                }
            }

            return height;
        }

        private void UpdateFieldValues(SerializedProperty property, int componentIndex)
        {
            if (componentIndex < 0 || componentIndex >= GameComponentsLookup.TotalComponents)
            {
                return;
            }

            Type componentType = GameComponentsLookup.componentTypes[componentIndex];
            SerializedProperty fieldValuesProp = property.FindPropertyRelative("fieldValues");

            fieldValuesProp.ClearArray();

            FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                fieldValuesProp.InsertArrayElementAtIndex(i);
                SerializedProperty fieldData = fieldValuesProp.GetArrayElementAtIndex(i);

                fieldData.FindPropertyRelative("fieldName").stringValue = field.Name;
                fieldData.FindPropertyRelative("fieldType").stringValue = field.FieldType.FullName;

                // Set default values based on type
                SetDefaultValue(fieldData, field.FieldType);
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        private void SetDefaultValue(SerializedProperty fieldData, Type fieldType)
        {
            if (fieldType == typeof(int))
            {
                fieldData.FindPropertyRelative("intValue").intValue = 0;
            }
            else if (fieldType == typeof(float))
            {
                fieldData.FindPropertyRelative("floatValue").floatValue = 0f;
            }
            else if (fieldType == typeof(bool))
            {
                fieldData.FindPropertyRelative("boolValue").boolValue = false;
            }
            else if (fieldType == typeof(string))
            {
                fieldData.FindPropertyRelative("stringValue").stringValue = "";
            }
            else if (fieldType == typeof(Vector3))
            {
                fieldData.FindPropertyRelative("vector3Value").vector3Value = Vector3.zero;
            }
            else if (fieldType == typeof(List<int>))
            {
                SerializedProperty listProp = fieldData.FindPropertyRelative("intListValue");
                listProp.ClearArray();
            }
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = fieldType.GetGenericArguments()[0];
                if (elementType.IsEnum)
                {
                    SerializedProperty listProp = fieldData.FindPropertyRelative("enumListValue");
                    listProp.ClearArray();
                }
            }
        }

        private void DrawComponentFields(SerializedProperty property, int componentIndex, Rect position, ref float yOffset)
        {
            SerializedProperty fieldValuesProp = property.FindPropertyRelative("fieldValues");

            // Check if fieldValues is empty and update if needed
            if (fieldValuesProp.arraySize == 0)
            {
                UpdateFieldValues(property, componentIndex);
            }

            for (int i = 0; i < fieldValuesProp.arraySize; i++)
            {
                SerializedProperty fieldData = fieldValuesProp.GetArrayElementAtIndex(i);
                string fieldName = fieldData.FindPropertyRelative("fieldName").stringValue;
                string fieldType = fieldData.FindPropertyRelative("fieldType").stringValue;

                if (fieldType.StartsWith("System.Collections.Generic.List`1[[System.Int32"))
                {
                    DrawListField(fieldData, fieldName, position, ref yOffset);
                }
                else if (fieldType.StartsWith("System.Collections.Generic.List`1[["))
                {
                    DrawEnumListField(fieldData, fieldName, fieldType, position, ref yOffset);
                }
                else
                {
                    Rect fieldRect = new Rect(position.x + 20, position.y + yOffset, position.width - 20, FieldHeight);
                    DrawFieldByType(fieldData, fieldName, fieldType, fieldRect);
                    yOffset += FieldHeight + Spacing;
                }
            }
        }

        private void DrawListField(SerializedProperty fieldData, string fieldName, Rect position, ref float yOffset)
        {
            SerializedProperty listProp = fieldData.FindPropertyRelative("intListValue");

            // Draw list header
            Rect headerRect = new Rect(position.x + 20, position.y + yOffset, position.width - 20, FieldHeight);
            EditorGUI.LabelField(headerRect, fieldName, $"List<int> (Size: {listProp.arraySize})");
            yOffset += FieldHeight + Spacing;

            // Draw list elements
            for (int i = 0; i < listProp.arraySize; i++)
            {
                Rect elementRect = new Rect(position.x + 40, position.y + yOffset, position.width - 80, ListElementHeight);
                Rect buttonRect = new Rect(position.x + position.width - 35, position.y + yOffset, 20, ListElementHeight);

                SerializedProperty element = listProp.GetArrayElementAtIndex(i);
                element.intValue = EditorGUI.IntField(elementRect, $"Element {i}", element.intValue);

                // Remove button
                if (GUI.Button(buttonRect, "-"))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                yOffset += ListElementHeight + Spacing;
            }

            // Add button
            Rect addButtonRect = new Rect(position.x + 40, position.y + yOffset, position.width - 60, FieldHeight);
            if (GUI.Button(addButtonRect, "+ Add Element"))
            {
                listProp.InsertArrayElementAtIndex(listProp.arraySize);
                if (listProp.arraySize > 0)
                {
                    listProp.GetArrayElementAtIndex(listProp.arraySize - 1).intValue = 0;
                }
            }
            yOffset += FieldHeight + Spacing;
        }

        private void DrawEnumListField(SerializedProperty fieldData, string fieldName, string fieldTypeName, Rect position, ref float yOffset)
        {
            SerializedProperty listProp = fieldData.FindPropertyRelative("enumListValue");

            // Extract enum type from the full type name
            Type enumType = GetEnumTypeFromListTypeName(fieldTypeName);
            if (enumType == null)
            {
                EditorGUI.LabelField(new Rect(position.x + 20, position.y + yOffset, position.width - 20, FieldHeight),
                    fieldName, "Unknown enum type");
                yOffset += FieldHeight + Spacing;
                return;
            }

            // Draw list header
            Rect headerRect = new Rect(position.x + 20, position.y + yOffset, position.width - 20, FieldHeight);
            EditorGUI.LabelField(headerRect, fieldName, $"List<{enumType.Name}> (Size: {listProp.arraySize})");
            yOffset += FieldHeight + Spacing;

            // Draw list elements
            for (int i = 0; i < listProp.arraySize; i++)
            {
                Rect elementRect = new Rect(position.x + 40, position.y + yOffset, position.width - 80, ListElementHeight);
                Rect buttonRect = new Rect(position.x + position.width - 35, position.y + yOffset, 20, ListElementHeight);

                SerializedProperty element = listProp.GetArrayElementAtIndex(i);
                Enum currentEnumValue = (Enum)Enum.ToObject(enumType, element.intValue);
                Enum newEnumValue = EditorGUI.EnumPopup(elementRect, $"Element {i}", currentEnumValue);
                element.intValue = Convert.ToInt32(newEnumValue);

                // Remove button
                if (GUI.Button(buttonRect, "-"))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                yOffset += ListElementHeight + Spacing;
            }

            // Add button
            Rect addButtonRect = new Rect(position.x + 40, position.y + yOffset, position.width - 60, FieldHeight);
            if (GUI.Button(addButtonRect, "+ Add Element"))
            {
                listProp.InsertArrayElementAtIndex(listProp.arraySize);
                if (listProp.arraySize > 0)
                {
                    listProp.GetArrayElementAtIndex(listProp.arraySize - 1).intValue = 0;
                }
            }
            yOffset += FieldHeight + Spacing;
        }

        private Type GetEnumTypeFromListTypeName(string fieldTypeName)
        {
            // Parse type name like "System.Collections.Generic.List`1[[Code.Gameplay.Produce.View.ItemsEnum, Assembly-CSharp, ...]]"
            int startIndex = fieldTypeName.IndexOf("[[") + 2;
            int endIndex = fieldTypeName.IndexOf(",", startIndex);

            if (startIndex > 1 && endIndex > startIndex)
            {
                string enumTypeName = fieldTypeName.Substring(startIndex, endIndex - startIndex);

                // Try to find the type in loaded assemblies
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type type = assembly.GetType(enumTypeName);
                    if (type != null && type.IsEnum)
                        return type;
                }
            }

            return null;
        }

        private void DrawFieldByType(SerializedProperty fieldData, string fieldName, string fieldTypeName, Rect rect)
        {
            Type fieldType = Type.GetType(fieldTypeName);

            if (fieldType == null)
            {
                // Try to find the type in loaded assemblies
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    fieldType = assembly.GetType(fieldTypeName);
                    if (fieldType != null) break;
                }
            }

            if (fieldType == null)
            {
                EditorGUI.LabelField(rect, fieldName, "Unknown type");
                return;
            }

            if (fieldType == typeof(int))
            {
                SerializedProperty valueProp = fieldData.FindPropertyRelative("intValue");
                valueProp.intValue = EditorGUI.IntField(rect, fieldName, valueProp.intValue);
            }
            else if (fieldType == typeof(float))
            {
                SerializedProperty valueProp = fieldData.FindPropertyRelative("floatValue");
                valueProp.floatValue = EditorGUI.FloatField(rect, fieldName, valueProp.floatValue);
            }
            else if (fieldType == typeof(bool))
            {
                SerializedProperty valueProp = fieldData.FindPropertyRelative("boolValue");
                valueProp.boolValue = EditorGUI.Toggle(rect, fieldName, valueProp.boolValue);
            }
            else if (fieldType == typeof(string))
            {
                SerializedProperty valueProp = fieldData.FindPropertyRelative("stringValue");
                valueProp.stringValue = EditorGUI.TextField(rect, fieldName, valueProp.stringValue);
            }
            else if (fieldType == typeof(Vector3))
            {
                SerializedProperty valueProp = fieldData.FindPropertyRelative("vector3Value");
                valueProp.vector3Value = EditorGUI.Vector3Field(rect, fieldName, valueProp.vector3Value);
            }
            else if (fieldType == typeof(List<int>))
            {
                // This case is handled in DrawComponentFields with custom layout
                // We don't draw it here because it needs multiple lines
            }
            else if (fieldType.IsEnum)
            {
                SerializedProperty valueProp = fieldData.FindPropertyRelative("intValue");
                valueProp.intValue = (int)(object)EditorGUI.EnumPopup(rect, fieldName, (Enum)Enum.ToObject(fieldType, valueProp.intValue));
            }
            else
            {
                EditorGUI.LabelField(rect, fieldName, $"Unsupported type: {fieldType.Name}");
            }
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
