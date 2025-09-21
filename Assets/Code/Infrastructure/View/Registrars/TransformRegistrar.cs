using Code.Gameplay.Common;
using UnityEngine;

namespace Code.Infrastructure.View.Registrars
{
    public class TransformRegistrar : EntityComponentRegistrar
    {
        public override void RegisterComponent() =>
            Entity.AddTransform(transform);

        public override void UnregisterComponent() =>
            Entity.RemoveTransform();
    }
}
