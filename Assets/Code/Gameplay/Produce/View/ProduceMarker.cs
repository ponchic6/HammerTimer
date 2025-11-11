using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Produce.View
{
    public class ProduceMarker : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;

        private readonly Collider[] _overlapResults = new Collider[4];
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;
        }

        private void Update()
        {
            bool isHoldingInteract = _game.inputEntity.isHoldingInteractInput;
            bool isInteractPressed = _game.inputEntity.isInteractDownInput;

            if (isInteractPressed && !_game.isProducingByPlayer && isHoldingInteract)
            {
                TryStartProduction();
                return;
            }

            if (!isHoldingInteract)
            {
                StopProduction();
                return;
            }

            if (_game.isProducingByPlayer)
            {
                ValidateProduction();
            }
        }

        private void TryStartProduction()
        {
            GameEntity socketEntity = FindSocketEntity();

            if (socketEntity != null && IsProduceMachine(socketEntity))
            {
                socketEntity.isProducingByPlayer = true;
            }
        }

        private void StopProduction()
        {
            if (_game.isProducingByPlayer)
            {
                _game.producingByPlayerEntity.isProducingByPlayer = false;
            }
        }

        private void ValidateProduction()
        {
            GameEntity socketEntity = FindSocketEntity();

            if (socketEntity == null || !IsProduceMachine(socketEntity))
            {
                _game.producingByPlayerEntity.isProducingByPlayer = false;
            }
        }

        private GameEntity FindSocketEntity()
        {
            Vector3 searchPosition = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(
                searchPosition,
                _interactionRadius,
                _overlapResults,
                _socketLayer
            );

            if (socketCount == 0)
                return null;

            EntityBehaviour entityBehaviour = _overlapResults[0].GetComponent<EntityBehaviour>();
            return entityBehaviour?.Entity;
        }

        private bool IsProduceMachine(GameEntity entity)
        {
            return entity.hasWorkbench || entity.hasProduceMachine || entity.hasAnvil;
        }
    }
}