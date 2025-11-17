using System;
using System.Collections.Generic;

namespace Code.Infrastructure.View.Extensions
{
    [Serializable]
    public class ComponentReference
    {
        public string componentName;
        public List<ComponentFieldData> fieldValues = new();

        public ComponentReference(string componentName = "")
        {
            this.componentName = componentName;
        }

        public int GetComponentIndex()
        {
            if (string.IsNullOrEmpty(componentName))
                return -1;

            for (int i = 0; i < GameComponentsLookup.componentNames.Length; i++)
            {
                if (GameComponentsLookup.componentNames[i] == componentName)
                    return i;
            }

            return -1;
        }
    }
}
