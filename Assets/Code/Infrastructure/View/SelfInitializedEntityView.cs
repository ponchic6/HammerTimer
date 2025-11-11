using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Code.Infrastructure.Services;
using Code.Infrastructure.View.Extensions;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.View
{
    public class SelfInitializedEntityView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _entityBehaviour;
        [SerializeField] private List<ComponentReference> _componentsToAdd = new();

        private IIdentifierService _identifiers;

        [Inject]
        private void Construct(IIdentifierService identifiers) =>
            _identifiers = identifiers;

        private void Awake()
        {
            GameEntity entity = Contexts.sharedInstance.game.CreateEntity();
            entity.AddId(_identifiers.Next());

            foreach (ComponentReference componentRef in _componentsToAdd)
            {
                int componentIndex = componentRef.componentIndex;
                if (componentIndex is >= 0 and < GameComponentsLookup.TotalComponents)
                {
                    IComponent component = entity.CreateComponent(componentIndex, GameComponentsLookup.componentTypes[componentIndex]);
                    
                    ApplyFieldValues(component, componentRef.fieldValues);

                    entity.AddComponent(componentIndex, component);
                }
            }

            _entityBehaviour.SetEntity(entity);
        }

        private void ApplyFieldValues(IComponent component, List<ComponentFieldData> fieldValues)
        {
            Type componentType = component.GetType();

            foreach (ComponentFieldData fieldData in fieldValues)
            {
                FieldInfo field = componentType.GetField(fieldData.fieldName, BindingFlags.Public | BindingFlags.Instance);

                if (field == null)
                    continue;

                try
                {
                    object value = GetFieldValue(fieldData, field.FieldType);
                    if (value != null)
                    {
                        field.SetValue(component, value);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to set field {fieldData.fieldName} on component {componentType.Name}: {ex.Message}");
                }
            }
        }

        private object GetFieldValue(ComponentFieldData fieldData, Type fieldType)
        {
            if (fieldType == typeof(int))
                return fieldData.intValue;

            if (fieldType == typeof(float))
                return fieldData.floatValue;

            if (fieldType == typeof(bool))
                return fieldData.boolValue;

            if (fieldType == typeof(string))
                return fieldData.stringValue;

            if (fieldType == typeof(Vector3))
                return fieldData.vector3Value;

            if (fieldType == typeof(List<int>))
                return fieldData.intListValue;

            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = fieldType.GetGenericArguments()[0];
                if (elementType.IsEnum)
                {
                    var list = (IList)Activator.CreateInstance(fieldType);
                    foreach (int enumValue in fieldData.enumListValue)
                    {
                        list.Add(Enum.ToObject(elementType, enumValue));
                    }
                    return list;
                }
            }

            if (fieldType.IsEnum)
                return Enum.ToObject(fieldType, fieldData.intValue);

            return null;
        }
    }
}