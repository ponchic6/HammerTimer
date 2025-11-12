using System.Collections.Generic;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Produce.Forge.View
{
    public class ForgeView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private List<ParticleSystem> fireParticles;
        [SerializeField] private Image forgePowerSlider;
        private bool _hasVfx;
        private CommonStaticData _commonStaticData;

        [Inject]
        public void Construct(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
        }

        private void Start()
        {
            fireParticles.ForEach(p => p.Stop());
        }
        
        private void Update()
        {
            forgePowerSlider.fillAmount = entityBehaviour.Entity.forge.Power / _commonStaticData.forgeMaxPower;

            if (Mathf.Approximately(entityBehaviour.Entity.forge.Power, 0f))
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

    }
}