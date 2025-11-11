using System;
using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Produce.Anvil.View
{
    public class AnvilView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private List<EnumVisualPair> anvilRecipes;
        [SerializeField] private List<EnumVisualPair> itemProgressBackgrounds;
        [SerializeField] private Image progressFiller;
        [SerializeField] private GameObject recipesContainer;
        [SerializeField] private float slideDownDuration;
        [SerializeField] private float slideDistance;
        private ItemsEnum? _currentRecipe;
        private bool _isAnimating;
        private bool _isShowingProgress;

        private void Start()
        {
            HideProgress();
        }

        private void Update()
        {
            bool hasProduceProgress = entityBehaviour.Entity.hasProduceProgress;

            if (hasProduceProgress)
            {
                if (!_isShowingProgress)
                {
                    ShowProgress();
                }

                UpdateProgress(entityBehaviour.Entity.produceProgress.Progress);
            }
            else
            {
                if (_isShowingProgress) 
                    HideProgress();
                
                if (entityBehaviour.Entity.hasGrabbedItem)
                {
                    recipesContainer.SetActive(false);
                    return;
                }

                if (!recipesContainer.activeSelf)
                    recipesContainer.SetActive(true);
                
                if (_currentRecipe == entityBehaviour.Entity.anvil.CurrentProduceItem)
                    return;

                if (_isAnimating)
                    return;
                
                ItemsEnum newRecipe = entityBehaviour.Entity.anvil.CurrentProduceItem;

                if (_currentRecipe.HasValue)
                    AnimateRecipeChange(_currentRecipe.Value, newRecipe);
                else
                    SetRecipeInstant(newRecipe);

                _currentRecipe = newRecipe;
            }
        }

        private void ShowProgress()
        {
            _isShowingProgress = true;
            recipesContainer.SetActive(false);
            ItemsEnum progressItemBackground = entityBehaviour.Entity.produceProgress.Item;
            itemProgressBackgrounds.ForEach(x => x.visual.SetActive(x.@enum == progressItemBackground));
            progressFiller.gameObject.SetActive(true);
        }

        private void HideProgress()
        {
            _isShowingProgress = false;
            itemProgressBackgrounds.ForEach(x => x.visual.SetActive(false));
            progressFiller.gameObject.SetActive(false);
            recipesContainer.SetActive(true);
        }

        private void UpdateProgress(float progress)
        {
            progressFiller.fillAmount = Mathf.Clamp01(progress);
        }

        private void SetRecipeInstant(ItemsEnum recipe)
        {
            foreach (var recipePair in anvilRecipes)
            {
                bool isActive = recipePair.@enum == recipe;
                recipePair.visual.SetActive(isActive);

                if (isActive)
                {
                    RectTransform rectTransform = recipePair.visual.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchoredPosition = Vector2.zero;
                    }
                }
            }
        }

        private void AnimateRecipeChange(ItemsEnum oldRecipe, ItemsEnum newRecipe)
        {
            _isAnimating = true;

            GameObject oldVisual = anvilRecipes.Find(x => x.@enum == oldRecipe)?.visual;
            GameObject newVisual = anvilRecipes.Find(x => x.@enum == newRecipe)?.visual;

            if (oldVisual == null || newVisual == null)
            {
                _isAnimating = false;
                return;
            }

            RectTransform oldRect = oldVisual.GetComponent<RectTransform>();
            RectTransform newRect = newVisual.GetComponent<RectTransform>();

            if (oldRect == null || newRect == null)
            {
                _isAnimating = false;
                return;
            }
            
            newVisual.SetActive(true);
            newRect.anchoredPosition = new Vector2(0, slideDistance);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(oldRect.DOAnchorPosY(-slideDistance, slideDownDuration).SetEase(Ease.InOutQuad));
            sequence.Join(newRect.DOAnchorPosY(0, slideDownDuration).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                oldVisual.SetActive(false);
                oldRect.anchoredPosition = Vector2.zero;
                _isAnimating = false;
            });
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}