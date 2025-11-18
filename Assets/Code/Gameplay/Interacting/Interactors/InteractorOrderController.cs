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
        private Collider[] _socketResults = new Collider[8];
        private Collider[] _grabbableResults = new Collider[8];
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
                GameEntity targetEntity = FindClosestSocketEntity();

                if (targetEntity != null)
                {
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
                GameEntity grabbableEntity = FindClosestGrabbableEntity();
                if (grabbableEntity != null)
                {
                    if (freeItemInteractor.TryGrabItem(_playerEntityBehavior, grabbableEntity))
                        return;
                }

                GameEntity socketEntity = FindClosestSocketEntity();
                if (socketEntity != null)
                {
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
            GameEntity socketEntity = FindClosestSocketEntity();
            if (socketEntity != null)
            {
                workbenchInteractor.TryClearWorkbench(socketEntity);
            }
        }

        private GameEntity FindClosestSocketEntity()
        {
            int size = Physics.OverlapSphereNonAlloc(transform.position, _interactionDistance, _socketResults, _socketLayer);

            if (size == 0)
                return null;

            GameEntity closestEntity = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < size; i++)
            {
                Collider collider = _socketResults[i];
                if (collider == null)
                    continue;

                EntityBehaviour entityBehaviour = collider.GetComponentInParent<EntityBehaviour>();
                if (entityBehaviour != null)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEntity = entityBehaviour.Entity;
                    }
                }
            }

            return closestEntity;
        }

        private GameEntity FindClosestGrabbableEntity()
        {
            int size = Physics.OverlapSphereNonAlloc(transform.position, _interactionDistance, _grabbableResults, _grabbableLayer);

            if (size == 0)
                return null;

            GameEntity closestEntity = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < size; i++)
            {
                Collider collider = _grabbableResults[i];
                if (collider == null)
                    continue;

                EntityBehaviour entityBehaviour = collider.GetComponentInParent<EntityBehaviour>();
                if (entityBehaviour != null)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEntity = entityBehaviour.Entity;
                    }
                }
            }

            return closestEntity;
        }
    }
}