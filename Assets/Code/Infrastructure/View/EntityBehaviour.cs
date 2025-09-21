using UnityEngine;

namespace Code.Infrastructure.View
{
    public class EntityBehaviour : MonoBehaviour
    {
        private GameEntity _entity;

        public GameEntity Entity => _entity;
        
        public void SetEntity(GameEntity entity)
        {
            _entity = entity;
            _entity.Retain(this);
            _entity.AddView(this);

            foreach (EntityComponentRegistrar registrar in GetComponentsInChildren<EntityComponentRegistrar>())
            {
                registrar.RegisterComponent();
            }
        }

        public void ReleaseEntity()
        {
            foreach (EntityComponentRegistrar registrar in GetComponentsInChildren<EntityComponentRegistrar>())
            {
                registrar.UnregisterComponent();
            }
            
            _entity.Release(this);
            _entity = null;
        }
    }
}