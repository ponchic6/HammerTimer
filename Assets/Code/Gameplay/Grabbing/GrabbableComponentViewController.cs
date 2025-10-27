using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Grabbing
{
    public class GrabbableComponentViewController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _entityBehaviour;
        [SerializeField] private CommonStaticData _commonStaticData;
        private GameObject _currentItemView;

        private void Start()
        {
            if (_entityBehaviour?.Entity != null && _entityBehaviour.Entity.hasGrabbableItem)
            {
                SpawnItemView();
            }
        }

        private void OnEnable()
        {
            if (_entityBehaviour?.Entity != null)
            {
                _entityBehaviour.Entity.OnComponentAdded += OnComponentAdded;
                _entityBehaviour.Entity.OnComponentReplaced += OnComponentReplaced;
                _entityBehaviour.Entity.OnComponentRemoved += OnComponentRemoved;
            }
        }

        private void OnDisable()
        {
            if (_entityBehaviour?.Entity != null)
            {
                _entityBehaviour.Entity.OnComponentAdded -= OnComponentAdded;
                _entityBehaviour.Entity.OnComponentReplaced -= OnComponentReplaced;
                _entityBehaviour.Entity.OnComponentRemoved -= OnComponentRemoved;
            }

            DestroyCurrentItemView();
        }

        private void OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.GrabbableItem)
            {
                SpawnItemView();
            }
        }

        private void OnComponentReplaced(IEntity entity, int index, IComponent previousComponent, IComponent newComponent)
        {
            if (index == GameComponentsLookup.GrabbableItem)
            {
                DestroyCurrentItemView();
                SpawnItemView();
            }
        }

        private void OnComponentRemoved(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.GrabbableItem)
            {
                DestroyCurrentItemView();
            }
        }

        private void SpawnItemView()
        {
            ItemsEnum itemType = _entityBehaviour.Entity.grabbableItem.Value;
            _commonStaticData.EnumPathPairs.TryGetValue(itemType, out string prefabPath);
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            
            _currentItemView = Instantiate(prefab, transform);
            _currentItemView.transform.localPosition = Vector3.zero;
            _currentItemView.transform.localRotation = Quaternion.identity;
        }

        private void DestroyCurrentItemView()
        {
            if (_currentItemView != null)
            {
                Destroy(_currentItemView);
                _currentItemView = null;
            }
        }
    }
}