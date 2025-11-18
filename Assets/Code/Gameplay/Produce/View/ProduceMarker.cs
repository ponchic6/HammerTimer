using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Produce.View
{
    public class ProduceMarker : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
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

            if (!_game.isProducingByPlayer && isHoldingInteract)
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
            RaycastHit hit;

            if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out hit, 1f, _socketLayer))
            {
                return hit.collider.GetComponent<EntityBehaviour>().Entity;
            }
            
            return null;
        }

        private bool IsProduceMachine(GameEntity entity)
        {
            return entity.hasWorkbench || entity.hasProduceMachine || entity.hasAnvil;
        }
    }
}