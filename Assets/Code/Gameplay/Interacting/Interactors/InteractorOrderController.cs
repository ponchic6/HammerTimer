using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Interacting.Interactors
{
    public class InteractorOrderController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _playerEntityBehavior;
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
        [SerializeField] private LayerMask _grabbableLayer;
        [SerializeField] private FreeItemInteractor freeItemInteractor;
        [SerializeField] private ShelfSocketInteractor shelfSocketInteractor;
        [SerializeField] private InfinityBoxInteractor infinityBoxInteractor;
        [SerializeField] private ProduceMachineInteractor produceMachineInteractor;
        [SerializeField] private WorkbenchInteractor workbenchInteractor;
        [SerializeField] private ForgeInteractor forgeInteractor;
        [SerializeField] private MouldingMachineInteractor mouldingMachineInteractor;
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
            Vector3 interactionPosition = transform.position + transform.forward;

            if (_playerEntityBehavior.Entity.hasGrabbedItem)
            {
                int socketCount = Physics.OverlapSphereNonAlloc(interactionPosition, _interactionRadius, _overlapResults, _socketLayer);

                if (socketCount <= 0)
                {
                    freeItemInteractor.TryReleaseItem(_playerEntityBehavior);
                    return;
                }

                GameEntity targetEntity = _overlapResults[0].GetComponentInParent<EntityBehaviour>().Entity;

                if (shelfSocketInteractor.TryReleaseItem(_playerEntityBehavior, targetEntity))
                    return;
                if (produceMachineInteractor.TryReleaseItem(_playerEntityBehavior, targetEntity))
                    return;
                if (workbenchInteractor.TryReleaseItem(_playerEntityBehavior, targetEntity))
                    return;
                if (forgeInteractor.TryReleaseItem(_playerEntityBehavior, targetEntity))
                    return;
                if (mouldingMachineInteractor.TryReleaseItem(_playerEntityBehavior, targetEntity))
                    return;
            }
            else
            {
                int grabbableCount = Physics.OverlapSphereNonAlloc(interactionPosition, _interactionRadius, _overlapResults, _grabbableLayer);

                if (grabbableCount > 0)
                {
                    GameEntity targetEntity = _overlapResults[0].GetComponentInParent<EntityBehaviour>().Entity;

                    if (freeItemInteractor.TryGrabItem(_playerEntityBehavior, targetEntity))
                        return;
                }

                int socketCount = Physics.OverlapSphereNonAlloc(interactionPosition, _interactionRadius, _overlapResults, _socketLayer);
                
                if (socketCount <= 0)
                    return;

                GameEntity socketEntity = _overlapResults[0].GetComponentInParent<EntityBehaviour>().Entity;

                if (shelfSocketInteractor.TryGrabItem(_playerEntityBehavior, socketEntity))
                    return;
                if (infinityBoxInteractor.TryGrabItem(_playerEntityBehavior, socketEntity))
                    return;
                if (produceMachineInteractor.TryGrabItem(_playerEntityBehavior, socketEntity))
                    return;
                if (workbenchInteractor.TryGrabItem(_playerEntityBehavior, socketEntity))
                    return;
                if (forgeInteractor.TryGrabItem(_playerEntityBehavior, socketEntity))
                    return;
                if (mouldingMachineInteractor.TryGrabItem(_playerEntityBehavior, socketEntity))
                    return;
            }
        }

        private void ProcessDoubleItemInteractions()
        {
            Vector3 interactionPosition = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(interactionPosition, _interactionRadius, _overlapResults, _socketLayer);

            if (socketCount <= 0)
                return;

            GameEntity socketEntity = _overlapResults[0].GetComponent<EntityBehaviour>().Entity;
            workbenchInteractor.TryClearWorkbench(socketEntity);
        }
    }
}