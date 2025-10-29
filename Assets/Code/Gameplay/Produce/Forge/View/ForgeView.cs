using System.Collections.Generic;
using Code.Infrastructure.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Produce.Forge.View
{
    public class ForgeView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private List<ParticleSystem> fireParticles;
        [SerializeField] private TMP_Text forgeTemperatureText;
        [SerializeField] private Image itemTemperatureSlider;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float updateInterval;
        private GameContext _game;
        private bool _hasVfx;

        private void Start()
        {
            _game = Contexts.sharedInstance.game;
        }

        private void Update()
        {
            forgeTemperatureText.SetText(entityBehaviour.Entity.forge.Temperature.ToString("F0"));
            UpdateGrabbedTemperature();

            if (Mathf.Approximately(entityBehaviour.Entity.forge.Temperature, 25))
            {
                if (!_hasVfx)
                    return;
                
                fireParticles.ForEach(p => p.Stop());
                _hasVfx = false;
            }
            else
            {
                if (_hasVfx)
                    return;
                
                fireParticles.ForEach(p => p.Play());
                _hasVfx = true;
            }
        }

        private void UpdateGrabbedTemperature()
        {
            if (entityBehaviour.Entity.hasGrabbedItem)
            {
                if (!backgroundImage.gameObject.activeSelf)
                {
                    backgroundImage.gameObject.SetActive(true);
                    itemTemperatureSlider.gameObject.SetActive(true);
                }
                
                float itemTemp = _game.GetEntityWithId(entityBehaviour.Entity.grabbedItem.Value).grabbableTemperature.Value;
                itemTemperatureSlider.fillAmount = itemTemp / 2000f;
            }
            else
            {
                if (!itemTemperatureSlider.transform.parent.gameObject.activeSelf)
                    return;
                
                itemTemperatureSlider.gameObject.SetActive(false);
                backgroundImage.gameObject.SetActive(false);
            }
        }
    }
}