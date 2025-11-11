using Code.Gameplay.Produce.View;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using DG.Tweening;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Produce.Shelf.View
{
    public class ShelfView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private Transform itemSocket;
        [SerializeField] private GameObject vfx;
        private CommonStaticData _commonStaticData;
        private GameObject _currentItemView;
        private GameContext _game;

        [Inject]
        public void Construct(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            vfx.SetActive(false);       
        }

        private void Start()
        {
            _game = Contexts.sharedInstance.game;
            entityBehaviour.Entity.OnComponentAdded += OnComponentAdded;
            entityBehaviour.Entity.OnComponentReplaced += OnComponentReplaced;
            entityBehaviour.Entity.OnComponentRemoved += OnComponentRemoved;
        }
        
        private void OnDestroy()
        {
            DestroyCurrentItemView();
        }

        private void RemoveSubscribers(IEntity entity)
        {
            entityBehaviour.Entity.OnComponentAdded -= OnComponentAdded;
            entityBehaviour.Entity.OnComponentReplaced -= OnComponentReplaced;
            entityBehaviour.Entity.OnComponentRemoved -= OnComponentRemoved;
        }
        
        private void OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.GrabbedItem)
            {
                SpawnItemView();
                vfx.SetActive(true);
            }

            if (index == GameComponentsLookup.Destructed)
            {
                RemoveSubscribers(entity);
            }
        }

        private void OnComponentReplaced(IEntity entity, int index, IComponent previousComponent, IComponent newComponent)
        {
            if (index == GameComponentsLookup.GrabbedItem)
            {
                DestroyCurrentItemView();
                SpawnItemView();
            }
        }

        private void OnComponentRemoved(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.GrabbedItem)
            {
                DestroyCurrentItemView();
                vfx.SetActive(false);
            }
        }
        
        private void SpawnItemView()
        {
            ItemsEnum itemType = _game.GetEntityWithId(entityBehaviour.Entity.grabbedItem.Value).grabbableItem.Value;
            _commonStaticData.EnumVisualPairs.TryGetValue(itemType, out GameObject visualPrefab);

            _currentItemView = Instantiate(visualPrefab, itemSocket);
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