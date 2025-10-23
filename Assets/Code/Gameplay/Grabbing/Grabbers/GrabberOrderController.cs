using System;
using Code.Infrastructure.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Grabbing.Grabbers
{
    public class GrabberOrderController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _playerEntityBehavior;
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
        [SerializeField] private LayerMask _grabbableLayer;
        [SerializeField] private FreeItemGrabber _freeItemGrabber;
        [SerializeField] private ProduceMachineGrabber _produceMachineGrabber;
        [SerializeField] private ShelfSocketGrabber _shelfSocketGrabber;
        [SerializeField] private WorkbenchGrabber _workbenchGrabber;
        [SerializeField] private ForgeGrabber _forgeGrabber;
        private Collider[] _overlapResults = new Collider[4];
        private GameContext _game;

        private void Start()
        {
            _game = Contexts.sharedInstance.game;
        }

        private void Update()
        {
            if (_game.inputEntity.isInteractDownInput) 
                ProcessSingleItemInteractions();

            if (_game.inputEntity.isDoubleInteractDownInput)
                ProcessDoubleItemInteractions();
        }

        private void ProcessSingleItemInteractions()
        {
            if (_playerEntityBehavior.Entity.hasGrabbedItem)
            {
                if (_freeItemGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_shelfSocketGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_produceMachineGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_workbenchGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_forgeGrabber.TryReleaseItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
            }
            else
            {
                if (_freeItemGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _grabbableLayer, _overlapResults))
                    return;
                if (_shelfSocketGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_produceMachineGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_workbenchGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
                if (_forgeGrabber.TryGrabItem(_playerEntityBehavior, _interactionRadius, _socketLayer, _overlapResults))
                    return;
            }
        }

        private void ProcessDoubleItemInteractions()
        {
            _workbenchGrabber.TryClearWorkbench(_interactionRadius, _socketLayer, _overlapResults);
        }
    }
}