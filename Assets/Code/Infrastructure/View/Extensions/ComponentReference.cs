using System;
using System.Collections.Generic;

namespace Code.Infrastructure.View.Extensions
{
    [Serializable]
    public class ComponentReference
    {
        public int componentIndex;
        public List<ComponentFieldData> fieldValues = new();

        public ComponentReference(int componentIndex = -1)
        {
            this.componentIndex = componentIndex;
        }
    }
}
