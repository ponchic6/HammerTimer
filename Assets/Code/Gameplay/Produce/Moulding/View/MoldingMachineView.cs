using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.View;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Produce.Moulding.View
{
    public class MoldingMachineView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private List<MoldEnumVisualPair> visuals;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject moltenIronPlane;
        [SerializeField] private GameObject qualityCursor;
        [SerializeField] private GameObject qualitySlider;
        [SerializeField] private float leftBounderX;
        [SerializeField] private float centerX;
        [SerializeField] private float rightBounderX;
        private MoldEnum? _currentMold;
        private CommonStaticData _commonStaticData;

        [Inject]
        public void Construct(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;       
        }
        
        private void Start()
        {
            entityBehaviour.Entity.OnComponentAdded += OnComponentAdded;
            entityBehaviour.Entity.OnComponentRemoved += OnComponentRemoved;
        }

        private void Update()
        {
            ProcessAnimation();
            UpdateMoldVisuals();
            UpdateQualityCursorPosition();
        }

        private void UpdateQualityCursorPosition()
        {
            if (entityBehaviour.Entity.hasMouldingQuality)
            {
                if (qualityCursor.transform.localPosition.x < centerX)
                {
                    Vector3 newLocPos = new Vector3(
                        leftBounderX + entityBehaviour.Entity.mouldingQuality.Quality * (centerX - leftBounderX),
                        qualityCursor.transform.localPosition.y,
                        qualityCursor.transform.localPosition.z);
                
                    qualityCursor.transform.localPosition = newLocPos;
                }
                else
                {
                    Vector3 newLocPos = new Vector3(
                        rightBounderX - entityBehaviour.Entity.mouldingQuality.Quality * (-centerX + rightBounderX),
                        qualityCursor.transform.localPosition.y,
                        qualityCursor.transform.localPosition.z);
                
                    qualityCursor.transform.localPosition = newLocPos;
                }
            }
            else
            {
                qualitySlider.SetActive(false);
            }
        }

        private void UpdateMoldVisuals()
        {
            if (entityBehaviour.Entity.mouldingMachine.MoldEnum == MoldEnum.NoMold)
            {
                if (!_currentMold.HasValue)
                    return;

                _currentMold = null;
                visuals.ForEach(x => { x.visual.SetActive(false); });
                return;
            }

            if (entityBehaviour.Entity.mouldingMachine.MoldEnum != MoldEnum.NoMold)
            {
                if (_currentMold.HasValue && _currentMold.Value == entityBehaviour.Entity.mouldingMachine.MoldEnum)
                    return;

                _currentMold = entityBehaviour.Entity.mouldingMachine.MoldEnum;
                visuals.ForEach(x => { x.visual.SetActive(x.@enum == _currentMold); });
            }
        }

        private void ProcessAnimation()
        {
            if (!entityBehaviour.Entity.hasMouldingQuality)
                return;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("MoldingAnimation") && stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
            {
                moltenIronPlane.SetActive(false);
            }
        }

        private void RemoveSubscribers(IEntity entity)
        {
            entityBehaviour.Entity.OnComponentAdded -= OnComponentAdded;
            entityBehaviour.Entity.OnComponentRemoved -= OnComponentRemoved;
        }

        private void OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.MouldingQuality)
            {
                StartMoldingAnimation();
                moltenIronPlane.SetActive(true);
                qualitySlider.SetActive(true);
                qualityCursor.transform.localPosition = new Vector3(leftBounderX, qualityCursor.transform.localPosition.y, qualityCursor.transform.localPosition.z);
            }
            
            if (index == GameComponentsLookup.Destructed) 
                RemoveSubscribers(entity);
        }

        private void OnComponentRemoved(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.MouldingQuality)
            {
                moltenIronPlane.SetActive(false);
                qualitySlider.SetActive(false);
                
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.normalizedTime < 0.92f) 
                    animator.Play("MoldingAnimation", 0, 0.92f);
            }
        }

        private void StartMoldingAnimation()
        {
            animator.Play("MoldingAnimation", 0, 0.0f);
        }
    }
}
