using System.Collections.Generic;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Produce.Forge.View
{
    public class ForgeView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private List<ParticleSystem> fireParticles;
        [SerializeField] private TMP_Text forgeTemperatureText;
        [SerializeField] private Image itemTemperatureSlider;
        [SerializeField] private Image backgroundImage;
        private GameContext _game;
        private bool _hasVfx;
        private CommonStaticData _commonStaticData;

        [Inject]
        public void Construct(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
        }

        private void Start()
        {
            _game = Contexts.sharedInstance.game;
        }

        private void Update()
        {
            int roundedTemp = Mathf.RoundToInt(entityBehaviour.Entity.forge.Temperature / 100f) * 100;
            forgeTemperatureText.SetText($"<mspace=0.34em>{roundedTemp}°C</mspace>");
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
                itemTemperatureSlider.fillAmount = itemTemp / _commonStaticData.meltingTemperature;
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