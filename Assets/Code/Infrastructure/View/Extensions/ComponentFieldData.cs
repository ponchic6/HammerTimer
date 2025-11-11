using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.View.Extensions
{
    [Serializable]
    public class ComponentFieldData
    {
        public string fieldName;
        public string fieldType;

        // Поддержка различных типов значений
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;
        public Vector3 vector3Value;
        public List<int> intListValue = new();
        public List<int> enumListValue = new();

        public ComponentFieldData(string name, string type)
        {
            fieldName = name;
            fieldType = type;
        }
    }
}
