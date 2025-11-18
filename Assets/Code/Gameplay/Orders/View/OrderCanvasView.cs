using System.Collections.Generic;
using Code.Infrastructure.StaticData;
using DG.Tweening;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Orders.View
{
    public class OrderCanvasView : MonoBehaviour
    {
        [SerializeField] private OrderView orderPrefabView;
        [SerializeField] private Transform orderContainer;
        [SerializeField] private float spacing;
        [SerializeField] private float slideInDuration;
        [SerializeField] private float shiftDuration;
        [SerializeField] private float slideInOffset;
        [SerializeField] private float slideOutDuration;
        [SerializeField] private float slideOutOffset;
        [SerializeField] private float horizontalOffset;
        [SerializeField] private float verticalOffset;

        private CommonStaticData _commonStaticData;
        private GameContext _game;
        private IGroup<GameEntity> _orders;
        private Dictionary<int, OrderView> _orderViews = new();

        [Inject]
        public void Construct(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
        }
        
        private void Start()
        {
            _game = Contexts.sharedInstance.game;
            _orders = _game.GetGroup(GameMatcher.Order);

            _orders.OnEntityAdded += OnOrderAdded;
        }

        private void Update()
        {
            foreach (GameEntity orderEntity in _orders)
            {
                if (_orderViews.TryGetValue(orderEntity.id.Value, out OrderView orderView)) 
                    orderView.UpdateTimer(orderEntity.order.Timer);
            }
        }

        private void OnDestroy()
        {
            _orders.OnEntityAdded -= OnOrderAdded;
        }

        private void OnOrderAdded(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            if (_orderViews.ContainsKey(entity.id.Value))
                return;

            ShiftExistingOrdersRight();

            OrderView orderView = Instantiate(orderPrefabView, orderContainer);
            orderView.SetupOrder(entity.id.Value);
            orderView.SetImage(_commonStaticData.itemSpritesData.spritesPairs.Find(x => x.Enum == entity.order.Item).Sprite);

            _orderViews[entity.id.Value] = orderView;

            RectTransform rectTransform = orderView.GetComponent<RectTransform>();
            Vector2 targetPosition = new Vector2(horizontalOffset, verticalOffset);
            Vector2 startPosition = new Vector2(horizontalOffset, verticalOffset + slideInOffset);

            rectTransform.anchoredPosition = startPosition;
            rectTransform.DOAnchorPos(targetPosition, slideInDuration).SetEase(Ease.OutBack);

            entity.OnComponentAdded += OnDestructedAdded;
        }

        private void ShiftExistingOrdersRight()
        {
            int currentIndex = 0;
            foreach (KeyValuePair<int, OrderView> kvp in _orderViews)
            {
                OrderView view = kvp.Value;
                RectTransform rectTransform = view.GetComponent<RectTransform>();

                currentIndex++;
                Vector2 newPosition = new Vector2(horizontalOffset + spacing * currentIndex, verticalOffset);

                if (DOTween.IsTweening(rectTransform))
                {
                    rectTransform.DOKill();
                    rectTransform.anchoredPosition = newPosition;
                }
                else
                {
                    rectTransform.DOAnchorPos(newPosition, shiftDuration).SetEase(Ease.OutQuad);
                }
            }
        }

        private void OnDestructedAdded(IEntity entity, int index, IComponent component)
        {
            if (index != GameComponentsLookup.SelfDestructTimer)
                return;

            entity.OnComponentAdded -= OnDestructedAdded;

            GameEntity gameEntity = (GameEntity)entity;

            if (_orderViews.TryGetValue(gameEntity.id.Value, out OrderView orderView))
            {
                _orderViews.Remove(gameEntity.id.Value);

                // Анимация уезжания вверх за пределы экрана
                RectTransform rectTransform = orderView.GetComponent<RectTransform>();
                Vector2 slideOutPosition = new Vector2(rectTransform.anchoredPosition.x, verticalOffset + slideOutOffset);

                rectTransform.DOAnchorPos(slideOutPosition, slideOutDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        if (orderView != null)
                            Destroy(orderView.gameObject);
                    });
            }
        }
    }
}