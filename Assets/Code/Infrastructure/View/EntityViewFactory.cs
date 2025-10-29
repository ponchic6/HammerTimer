using UnityEngine;
using Zenject;
using Code.Infrastructure.View.Registrars;

namespace Code.Infrastructure.View
{
    public class EntityViewFactory : IEntityViewFactory
    {
        private DiContainer _diContainer;

        public EntityViewFactory(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public EntityBehaviour CreateViewForEntityFromPath(GameEntity gameEntity)
        {
            var view = _diContainer.InstantiatePrefabResource(gameEntity.viewPath.Value);
            var entityBehaviour = view.GetComponent<EntityBehaviour>();
            if (entityBehaviour == null)
                entityBehaviour = view.AddComponent<EntityBehaviour>();
            EnsureTransformRegistrar(view, entityBehaviour);
            entityBehaviour.SetEntity(gameEntity);
            return entityBehaviour;
        }
        
        public EntityBehaviour CreateViewForEntityFromPrefab(GameEntity gameEntity)
        {
            GameObject view;

            if (gameEntity.hasViewPrefabWithParent)
                view = _diContainer.InstantiatePrefab(gameEntity.viewPrefabWithParent.Value, gameEntity.viewPrefabWithParent.Parent.transform);
            else
                view = _diContainer.InstantiatePrefab(gameEntity.viewPrefab.Value);
            
            var entityBehaviour = view.GetComponent<EntityBehaviour>();
            if (entityBehaviour == null)
                entityBehaviour = view.AddComponent<EntityBehaviour>();
            EnsureTransformRegistrar(view, entityBehaviour);
            entityBehaviour.SetEntity(gameEntity);
            return entityBehaviour;
        }
        
        private static void EnsureTransformRegistrar(GameObject view, EntityBehaviour entityBehaviour)
        {
            var registrar = view.GetComponent<TransformRegistrar>();
            if (registrar == null)
                registrar = view.AddComponent<TransformRegistrar>();
        }
    }
}