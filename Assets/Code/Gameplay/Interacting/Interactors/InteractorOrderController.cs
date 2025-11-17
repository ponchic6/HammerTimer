using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Interacting.Interactors
{
    public class InteractorOrderController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _playerEntityBehavior;
        [SerializeField] private float _interactionDistance;
        [SerializeField] private LayerMask _socketLayer;
        [SerializeField] private LayerMask _grabbableLayer;
        [SerializeField] private FreeItemInteractor freeItemInteractor;
        [SerializeField] private ShelfSocketInteractor shelfSocketInteractor;
        [SerializeField] private InfinityBoxInteractor infinityBoxInteractor;
        [SerializeField] private ProduceMachineInteractor produceMachineInteractor;
        [SerializeField] private WorkbenchInteractor workbenchInteractor;
        [SerializeField] private ForgeInteractor forgeInteractor;
        [SerializeField] private MouldingMachineInteractor mouldingMachineInteractor;
        [SerializeField] private AnvilInteractor anvilInteractor;
        [SerializeField] private OrderReleaseInteractor orderReleaseInteractor;
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
                RaycastHit hit;
                
                if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out hit, _interactionDistance, _socketLayer))
                {
                    GameEntity targetEntity = hit.collider.GetComponentInParent<EntityBehaviour>().Entity;

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
                    if (anvilInteractor.TryInteractWithItem(_playerEntityBehavior, targetEntity))
                        return;
                    if (orderReleaseInteractor.TryReleaseItem(_playerEntityBehavior, targetEntity))
                        return;
                }
                else
                {
                    freeItemInteractor.TryReleaseItem(_playerEntityBehavior);
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out hit, _interactionDistance, _grabbableLayer))
                {
                    GameEntity targetEntity = hit.collider.GetComponentInParent<EntityBehaviour>().Entity;

                    if (freeItemInteractor.TryGrabItem(_playerEntityBehavior, targetEntity))
                        return;
                }

                if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out hit, _interactionDistance, _socketLayer))
                {
                    GameEntity socketEntity = hit.collider.GetComponentInParent<EntityBehaviour>().Entity;

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
                    if (anvilInteractor.TryInteractWithoutItem(_playerEntityBehavior, socketEntity))
                        return;
                }
            }
        }

        private void ProcessDoubleItemInteractions()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, _interactionDistance, _socketLayer))
            {
                GameEntity socketEntity = hit.collider.GetComponent<EntityBehaviour>().Entity;
                workbenchInteractor.TryClearWorkbench(socketEntity);
            }
        }
    }
}