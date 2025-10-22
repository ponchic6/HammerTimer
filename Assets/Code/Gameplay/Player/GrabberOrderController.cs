using Code.Infrastructure.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player
{
    public class GrabberOrderController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _playerEntityBehavior;
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
        [SerializeField] private LayerMask _grabbableLayer;
        [SerializeField] private PlayerFreeItemGrabber _playerFreeItemGrabber;
        [SerializeField] private PlayerProduceMachineGrabber _playerProduceMachineGrabber;
        [SerializeField] private PlayerShelfSocketGrabber _playerShelfSocketGrabber;
        private Collider[] _overlapResults = new Collider[4];
        private IReadOnlyInputService _inputService;

        [Inject]
        public void Construct(IReadOnlyInputService inputService)
        {
            _inputService = inputService;
        }
        
        private void Update()
        {
            if (!_inputService.GetInteractKeyDown())
                return;

            if (_playerEntityBehavior.Entity.hasGrabbedItem)
            {
                if (_playerFreeItemGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_playerShelfSocketGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_playerProduceMachineGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
            }
            else
            {
                if (_playerFreeItemGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _grabbableLayer, _overlapResults))
                    return;
                if (_playerShelfSocketGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_playerProduceMachineGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
            }
        }
    }
}