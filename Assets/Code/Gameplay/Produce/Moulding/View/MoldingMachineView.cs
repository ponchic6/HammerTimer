using System.Collections.Generic;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.View;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Moulding.View
{
    public class MoldingMachineView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private List<MoldEnumVisualPair> visuals;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject moltenIronPlane;
        private MoldEnum? _currentMold = null;

        private void Start()
        {
            entityBehaviour.Entity.OnComponentAdded += OnComponentAdded;
        }

        private void Update()
        {
            if (entityBehaviour.Entity.mouldingMachine.MoldEnumValue == MoldEnum.NoMold)
            {
                if (!_currentMold.HasValue)
                    return;
                
                _currentMold = null;
                visuals.ForEach(x => { x.visual.SetActive(false); });
                return;
            }

            if (entityBehaviour.Entity.mouldingMachine.MoldEnumValue != MoldEnum.NoMold)
            {
                if (_currentMold.HasValue && _currentMold.Value == entityBehaviour.Entity.mouldingMachine.MoldEnumValue)
                    return;
                
                _currentMold = entityBehaviour.Entity.mouldingMachine.MoldEnumValue;
                visuals.ForEach(x => { x.visual.SetActive(x.@enum == _currentMold); });
            }
        }

        private void OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            if (index == GameComponentsLookup.MouldingQuality)
            {
                StartMoldingAnimation();
                moltenIronPlane.SetActive(true);
            }

            if (index == GameComponentsLookup.Destructed) 
                RemoveSubscribers(entity);
        }

        private void RemoveSubscribers(IEntity entity)
        {
            entityBehaviour.Entity.OnComponentAdded -= OnComponentAdded;
        }

        private void StartMoldingAnimation() =>
            animator.Play("MoldingAnimation");
    }
}
