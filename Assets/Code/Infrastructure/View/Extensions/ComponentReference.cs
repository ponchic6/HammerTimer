using System;

namespace Code.Infrastructure.View.Extensions
{
    [Serializable]
    public class ComponentReference
    {
        public int componentIndex;

        public ComponentReference(int componentIndex = -1)
        {
            this.componentIndex = componentIndex;       
        }
    }
}
