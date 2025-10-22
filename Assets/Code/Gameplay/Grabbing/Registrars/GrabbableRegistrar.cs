using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Grabbing.Registrars
{
    public class GrabbableRegistrar : EntityComponentRegistrar
    {
        public GrabbableEnum grabbableEnum;
    
        public override void RegisterComponent() =>
            Entity.AddGrabbableItem(grabbableEnum);

        public override void UnregisterComponent() =>
            Entity.RemoveGrabbableItem();
    }
}