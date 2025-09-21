using UnityEngine;

namespace Code.Infrastructure.View
{
    public abstract class EntityComponentRegistrar : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _entityBehaviour;

        protected GameEntity Entity => _entityBehaviour.Entity;
        protected EntityBehaviour EntityBehaviour => _entityBehaviour;

        protected virtual void Awake()
        {
            if (_entityBehaviour == null)
                _entityBehaviour = GetComponentInParent<EntityBehaviour>();
        }

        public abstract void RegisterComponent();
        public abstract void UnregisterComponent();
    }
}