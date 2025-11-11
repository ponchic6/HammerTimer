using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.View
{
    public class GrabbableComponentViewController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _entityBehaviour;
        [SerializeField] private CommonStaticData _commonStaticData;
        private GameObject _currentItemView;
        
        private void Start()
        {
            _entityBehaviour.Entity.OnComponentAdded += OnComponentAdded;
            _entityBehaviour.Entity.OnComponentReplaced += OnComponentReplaced;
            _entityBehaviour.Entity.OnComponentRemoved += OnComponentRemoved;
            SpawnItemView();
        }

        private void OnDestroy()
        {
            DestroyCurrentItemView();
        }

        private void RemoveSubscribers(IEntity entity)
        {
            _entityBehaviour.Entity.OnComponentAdded -= OnComponentAdded;
            _entityBehaviour.Entity.OnComponentReplaced -= OnComponentReplaced;
            _entityBehaviour.Entity.OnComponentRemoved -= OnComponentRemoved;
        }

        private void OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.GrabbableItem)
            {
                SpawnItemView();
            }

            if (index == GameComponentsLookup.Destructed)
            {
                RemoveSubscribers(entity);
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
            _commonStaticData.EnumVisualPairs.TryGetValue(itemType, out GameObject visualPrefab);

            _currentItemView = Instantiate(visualPrefab, transform);
            _currentItemView.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            _currentItemView.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);

            _currentItemView.transform.DOLocalRotate(new Vector3(20f, 360f, 0f), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            _currentItemView.transform.DOLocalMoveY(0.5f, 1.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void DestroyCurrentItemView()
        {
            if (_currentItemView != null)
            {
                _currentItemView.transform.DOKill();
                Destroy(_currentItemView);
                _currentItemView = null;
            }
        }
    }
}