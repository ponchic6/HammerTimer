using System.Collections.Generic;
using Code.Infrastructure.Services;
using Code.Infrastructure.View.Extensions;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.View
{
    public class SelfInitializedEntityView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _entityBehaviour;
        [SerializeField] private List<ComponentReference> _componentsToAdd = new();

        private IIdentifierService _identifiers;

        [Inject]
        private void Construct(IIdentifierService identifiers) =>
            _identifiers = identifiers;

        private void Awake()
        {
            GameEntity entity = Contexts.sharedInstance.game.CreateEntity();
            entity.AddId(_identifiers.Next());

            foreach (ComponentReference componentRef in _componentsToAdd)
            {
                int componentIndex = componentRef.componentIndex;
                if (componentIndex is >= 0 and < GameComponentsLookup.TotalComponents)
                {
                    IComponent component = entity.CreateComponent(componentIndex, GameComponentsLookup.componentTypes[componentIndex]);
                    entity.AddComponent(componentIndex, component);
                }
            }

            _entityBehaviour.SetEntity(entity);
        }
    }
}