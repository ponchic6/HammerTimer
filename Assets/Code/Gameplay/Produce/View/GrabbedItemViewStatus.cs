using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Produce.View
{
    public class GrabbedItemViewStatus : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private Image itemSprite;
        [SerializeField] private TMP_Text temperatureText;
        [SerializeField] private GrabbableOrGrabbedItem grabbableOrGrabbedItem;
        [SerializeField] private float originalScale;
        private ItemsEnum? _currentItem;
        private GameContext _game;
        private CommonStaticData _staticData;

        [Inject]
        public void Construct(CommonStaticData staticData)
        {
            _staticData = staticData;
        }

        private void Start()
        {
            _game = Contexts.sharedInstance.game;

            itemSprite.gameObject.SetActive(false);
            temperatureText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (_currentItem != null) 
                ShowVisual(_currentItem.Value);

            if (entityBehaviour.Entity != null && entityBehaviour.Entity.hasGrabbableTemperature)
            {
                string tempText = $"<mspace=0.34em>{entityBehaviour.Entity.grabbableTemperature.Value:F0}°C</mspace>";
                ShowTemperatureText(tempText);
            }
                
        }

        private void Update()
        {
            if (grabbableOrGrabbedItem == GrabbableOrGrabbedItem.Grabbable)
            {
                if (entityBehaviour.Entity.hasGrabbableTemperature)
                {
                    string tempText = $"<mspace=0.34em>{entityBehaviour.Entity.grabbableTemperature.Value:F0}°C</mspace>";
                    if (!temperatureText.gameObject.activeSelf)
                        ShowTemperatureText(tempText);
                    else
                        temperatureText.SetText(tempText);
                }
                else
                {
                    if (temperatureText.gameObject.activeSelf)
                        HideTemperatureText();
                }

                if (_currentItem == entityBehaviour.Entity.grabbableItem.Value)
                {
                    return;
                }

                ItemsEnum? previousItem = _currentItem;
                _currentItem = entityBehaviour.Entity.grabbableItem.Value;

                if (previousItem.HasValue)
                    HideVisual(previousItem.Value, () => ShowVisual(_currentItem.Value));
                else
                    ShowVisual(_currentItem.Value);
            }

            if (grabbableOrGrabbedItem == GrabbableOrGrabbedItem.GrabbedItem)
            {
                if (!entityBehaviour.Entity.hasGrabbedItem)
                {
                    if (!_currentItem.HasValue)
                        return;

                    ItemsEnum itemToHide = _currentItem.Value;
                    _currentItem = null;

                    HideVisual(itemToHide);
                    if (temperatureText.gameObject.activeSelf)
                        HideTemperatureText();
                    return;
                }

                GameEntity grabbableItem = _game.GetEntityWithId(entityBehaviour.Entity.grabbedItem.Value);

                if (grabbableItem.hasGrabbableTemperature)
                {
                    string tempText = $"<mspace=0.34em>{grabbableItem.grabbableTemperature.Value:F0}°C</mspace>";
                    if (!temperatureText.gameObject.activeSelf)
                        ShowTemperatureText(tempText);
                    else
                        temperatureText.SetText(tempText);
                }
                else
                {
                    if (temperatureText.gameObject.activeSelf)
                        HideTemperatureText();
                }

                if (_currentItem == grabbableItem.grabbableItem.Value)
                    return;

                ItemsEnum? previousItem = _currentItem;
                _currentItem = grabbableItem.grabbableItem.Value;

                if (previousItem.HasValue)
                    HideVisual(previousItem.Value, () => ShowVisual(_currentItem.Value));
                else
                    ShowVisual(_currentItem.Value);
            }
        }

        private void ShowVisual(ItemsEnum itemType)
        {
            ItemEnumSpritesPair spritePair = _staticData.itemSpritesData.spritesPairs
                .Find(x => x.Enum == itemType);

            if (spritePair == null || spritePair.Sprite == null)
                return;

            itemSprite.sprite = spritePair.Sprite;
            itemSprite.transform.DOKill();
            itemSprite.gameObject.SetActive(true);
            itemSprite.transform.localScale = Vector3.zero;
            itemSprite.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);
        }

        private void HideVisual(ItemsEnum itemType, System.Action onComplete = null)
        {
            if (!itemSprite.gameObject.activeSelf)
            {
                onComplete?.Invoke();
                return;
            }

            itemSprite.transform.DOKill();
            itemSprite.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    itemSprite.gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
        }

        private void ShowTemperatureText(string text)
        {
            temperatureText.transform.DOKill();
            temperatureText.gameObject.SetActive(true);
            temperatureText.SetText(text);
            temperatureText.transform.localScale = Vector3.zero;
            temperatureText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        private void HideTemperatureText()
        {
            temperatureText.transform.DOKill();
            temperatureText.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    temperatureText.gameObject.SetActive(false);
                });
        }
    }

    public enum GrabbableOrGrabbedItem
    {
        Grabbable,
        GrabbedItem
    }
}