using Code.Gameplay.Grabbing;
using Code.Gameplay.Grabbing.Registrars;
using Code.Gameplay.Grabbing.Services;
using Code.Infrastructure.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player
{
    public class PlayerGrabber : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _playerEntityBehavior;
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _grabbableLayer;
        [SerializeField] private LayerMask _socketLayer;
        private Collider[] _overlapResults = new Collider[4];

        private IReadOnlyInputService _inputService;
        private IGrabbableFactory _grabbableFactory;

        [Inject]
        public void Construct(IReadOnlyInputService inputService, IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
            _inputService = inputService;
        }

        private void Update()
        {
            if (!_inputService.GetInteractKeyDown())
                return;

            if (_playerEntityBehavior.Entity.hasGrabbedItem)
                ReleaseItem();
            else
                TryGrabItem();
        }

        private void TryGrabItem()
        {
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, _interactionRadius, _overlapResults, _socketLayer);
            
            if (socketCount > 0)
            {
                Collider socketCollider = _overlapResults[0];
                GameEntity socketEntity = socketCollider.GetComponent<EntityBehaviour>().Entity;
                
                if (!socketEntity.isSocket || socketEntity.hasProduceProgress || !socketEntity.hasGrabbedItem)
                    return;
                
                GrabbableEnum grabbableEnum = socketEntity.grabbedItem.Value;
                _playerEntityBehavior.Entity.AddGrabbedItem(grabbableEnum);
                socketEntity.RemoveGrabbedItem();
                return;

            }

            int grabbableCount = Physics.OverlapSphereNonAlloc(position, _interactionRadius, _overlapResults, _grabbableLayer);
            
            if (grabbableCount > 0)
            {
                GrabbableRegistrar itemToGrab = _overlapResults[0].GetComponent<GrabbableRegistrar>();
                GameEntity grabbaleEntity = itemToGrab.GetComponent<EntityBehaviour>().Entity;
                
                if (!grabbaleEntity.hasGrabbableItem)
                    return;
                
                _playerEntityBehavior.Entity.AddGrabbedItem(itemToGrab.grabbableEnum);
                grabbaleEntity.isDestructed = true;
            }
        }

        private void ReleaseItem()
        {
            GrabbableEnum grabbableEnum = _playerEntityBehavior.Entity.grabbedItem.Value;
            
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, _interactionRadius, _overlapResults, _socketLayer);
            
            if (socketCount > 0)
            {
                GameEntity socketEntity = _overlapResults[0].GetComponent<EntityBehaviour>().Entity;

                if (!socketEntity.isSocket || socketEntity.hasGrabbedItem)
                {
                    _grabbableFactory.SpawnNearWithPlayer(grabbableEnum);
                }
                else
                {
                    socketEntity.AddGrabbedItem(grabbableEnum);
                }
            }
            else
            {
                _grabbableFactory.SpawnNearWithPlayer(grabbableEnum);
            }

            _playerEntityBehavior.Entity.RemoveGrabbedItem();
        }
    }
}