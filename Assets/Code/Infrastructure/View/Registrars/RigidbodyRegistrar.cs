using UnityEngine;

namespace Code.Infrastructure.View.Registrars
{
    public class RigidbodyRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private Rigidbody rigidbodyComponent;
        
        public override void RegisterComponent() =>
            Entity.AddRigidbody(rigidbodyComponent);

        public override void UnregisterComponent() =>
            Entity.RemoveRigidbody();
    }
}