using System;
using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using DG.Tweening;
using Entitas;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Produce.Workbench.View
{
    public class WorkbenchView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private Canvas canvas;
        [SerializeField] private float radius;
        [SerializeField] private float collapseDuration;
        [SerializeField] private Image progressSlider;
        [SerializeField] private Image spriteTemplate;
        private readonly List<Image> _activeSprites = new();
        private int _lastItemCount;
        private GameContext _game;
        private Image _resultSprite;
        private bool _isAnimating;
        private Vector3 _progressSliderOriginalScale;
        private CommonStaticData _commonStaticData;
        private Dictionary<ItemsEnum, Sprite> _itemSprites;

        [Inject]
        public void Construct(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
        }

        private void Start()
        {
            _game = Contexts.sharedInstance.game;

            InitializeItemSprites();
            spriteTemplate.gameObject.SetActive(false);

            progressSlider.gameObject.SetActive(false);
            _progressSliderOriginalScale = progressSlider.transform.localScale;

            _lastItemCount = entityBehaviour.Entity.workbench.Value.Count;
            UpdateSprites();

            entityBehaviour.Entity.OnComponentAdded += OnComponentAdded;
            entityBehaviour.Entity.OnComponentRemoved += OnComponentRemoved;
        }

        private void InitializeItemSprites()
        {
            _itemSprites = new Dictionary<ItemsEnum, Sprite>();
            foreach (var pair in _commonStaticData.itemSpritesData.spritesPairs)
            {
                if (!_itemSprites.ContainsKey(pair.Enum))
                {
                    _itemSprites.Add(pair.Enum, pair.Sprite);
                }
            }
        }

        private void OnDestroy()
        {
            entityBehaviour.Entity.OnComponentAdded -= OnComponentAdded;
            entityBehaviour.Entity.OnComponentRemoved -= OnComponentRemoved;
        }

        private void Update()
        {
            if (entityBehaviour.Entity.hasProduceProgress)
            {
                UpdateProgress();
            }
            else
            {
                int currentCount = entityBehaviour.Entity.workbench.Value.Count;

                if (currentCount != _lastItemCount)
                {
                    _lastItemCount = currentCount;
                    UpdateSprites();
                }
            }
        }

        private void OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.ProduceProgress) 
                OnProduceProgressAdded();
        }

        private void OnComponentRemoved(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.ProduceProgress)
                OnProduceProgressRemoved();

            if (index == GameComponentsLookup.GrabbedItem)
                OnGrabbedItemRemoved();
        }

        private void UpdateSprites()
        {
            if (!entityBehaviour.Entity.hasWorkbench)
            {
                ClearAllSprites();
                return;
            }

            List<int> workbenchItems = entityBehaviour.Entity.workbench.Value;

            if (workbenchItems.Count > _activeSprites.Count)
            {
                for (int i = _activeSprites.Count; i < workbenchItems.Count; i++)
                {
                    int itemId = workbenchItems[i];
                    GameEntity grabbableEntity = _game.GetEntityWithId(itemId);
                    ItemsEnum itemType = grabbableEntity.grabbableItem.Value;
                    AddSprite(itemType);
                }
                RepositionSprites();
            }
            else if (workbenchItems.Count < _activeSprites.Count)
            {
                for (int i = _activeSprites.Count - 1; i >= workbenchItems.Count; i--)
                {
                    RemoveSprite(i);
                }
                RepositionSprites();
            }
        }

        private void AddSprite(ItemsEnum itemType)
        {
            if (!_itemSprites.TryGetValue(itemType, out Sprite sprite) || sprite == null)
                return;

            Image spriteInstance = Instantiate(spriteTemplate, canvas.transform);
            spriteInstance.sprite = sprite;
            Vector3 originalScale = spriteInstance.transform.localScale;
            spriteInstance.transform.localScale = Vector3.zero;
            spriteInstance.gameObject.SetActive(true);

            _activeSprites.Add(spriteInstance);

            _isAnimating = true;
            spriteInstance.transform.DOScale(originalScale, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _isAnimating = false);
        }

        private void RemoveSprite(int index)
        {
            if (index < 0 || index >= _activeSprites.Count)
                return;

            Image spriteObj = _activeSprites[index];
            _activeSprites.RemoveAt(index);

            spriteObj.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(spriteObj.gameObject));
        }

        private void RepositionSprites()
        {
            int count = _activeSprites.Count;

            if (count == 0)
                return;

            _isAnimating = true;

            if (count == 1)
            {
                _activeSprites[0].transform.DOLocalMove(Vector3.zero, 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => _isAnimating = false);
                return;
            }

            float angleStep = 360f / count;
            float startAngle = -90f;

            for (int i = 0; i < count; i++)
            {
                float angle = startAngle + angleStep * i;
                float radian = angle * Mathf.Deg2Rad;

                Vector3 targetPosition = new Vector3(
                    Mathf.Cos(radian) * radius,
                    Mathf.Sin(radian) * radius,
                    0f
                );

                Tween tween = _activeSprites[i].transform.DOLocalMove(targetPosition, 0.3f).SetEase(Ease.OutQuad);

                if (i == count - 1)
                {
                    tween.OnComplete(() => _isAnimating = false);
                }
            }
        }

        private void ClearAllSprites()
        {
            foreach (Image sprite in _activeSprites)
            {
                if (sprite != null)
                {
                    sprite.transform.DOKill();
                    Destroy(sprite.gameObject);
                }
            }

            _activeSprites.Clear();
        }

        private void OnProduceProgressAdded()
        {
            GameEntity entity = entityBehaviour.Entity;
            ItemsEnum resultItemType = entity.produceProgress.Item;

            if (_isAnimating)
            {
                DOVirtual.DelayedCall(0.05f, OnProduceProgressAdded);
                return;
            }

            CollapseSpritesToCenter(() =>
            {
                ShowResultSprite(resultItemType);
            });
        }

        private void OnProduceProgressRemoved()
        {
            if (progressSlider.gameObject.activeSelf)
            {
                progressSlider.transform.DOScale(Vector3.zero, 0.2f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        progressSlider.gameObject.SetActive(false);
                        PlayItemReadyAnimation();
                    });
            }
        }

        private void OnGrabbedItemRemoved()
        {
            if (_resultSprite != null)
            {
                _resultSprite.transform.DOKill();
                _resultSprite.transform.DOScale(Vector3.zero, 0.2f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        Destroy(_resultSprite.gameObject);
                        _resultSprite = null;
                    });
            }
        }

        private void PlayItemReadyAnimation()
        {
            if (_resultSprite == null)
                return;

            Vector3 originalScale = _resultSprite.transform.localScale;
            Vector3 breatheScale = originalScale * 1.15f;

            _resultSprite.transform.DOKill();
            _resultSprite.transform.DOScale(breatheScale, 0.15f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _resultSprite.transform.DOScale(originalScale, 0.15f)
                        .SetEase(Ease.InQuad);
                });
        }

        private void CollapseSpritesToCenter(Action onComplete)
        {
            if (_activeSprites.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            int completedAnimations = 0;

            foreach (Image sprite in _activeSprites)
            {
                sprite.transform.DOLocalMove(Vector3.zero, collapseDuration)
                    .SetEase(Ease.InQuad);

                sprite.transform.DOScale(Vector3.zero, collapseDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        completedAnimations++;
                        if (completedAnimations >= _activeSprites.Count)
                        {
                            ClearAllSprites();
                            onComplete?.Invoke();
                        }
                    });
            }
        }

        private void ShowResultSprite(ItemsEnum itemType)
        {
            if (!_itemSprites.TryGetValue(itemType, out Sprite sprite) || sprite == null)
                return;

            _resultSprite = Instantiate(spriteTemplate, canvas.transform);
            _resultSprite.sprite = sprite;
            Vector3 originalScale = spriteTemplate.transform.localScale;
            _resultSprite.transform.localPosition = Vector3.zero;
            _resultSprite.transform.localScale = Vector3.zero;
            _resultSprite.gameObject.SetActive(true);

            _resultSprite.transform.DOScale(originalScale, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => ShowProgressSlider());
        }

        private void ShowProgressSlider()
        {
            progressSlider.fillAmount = 0f;
            progressSlider.gameObject.SetActive(true);

            progressSlider.transform.localScale = Vector3.zero;

            progressSlider.transform.DOScale(_progressSliderOriginalScale, 0.3f)
                .SetEase(Ease.OutBack);
        }

        private void UpdateProgress()
        {
            if (!entityBehaviour.Entity.hasProduceProgress)
                return;

            float currentProgress = entityBehaviour.Entity.produceProgress.Progress;
            progressSlider.fillAmount = currentProgress;
        }
    }
}