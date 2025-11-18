using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Produce.View
{
    public class ProduceMarker : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
        private Collider[] _results = new Collider[4];
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;
        }

        private void Update()
        {
            bool isHoldingInteract = _game.inputEntity.isHoldingInteractInput;

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
            int size = Physics.OverlapSphereNonAlloc(transform.position, _interactionRadius, _results, _socketLayer);

            if (size == 0)
                return null;

            GameEntity closestEntity = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < size; i++)
            {
                Collider collider = _results[i];
                if (collider == null)
                    continue;

                EntityBehaviour entityBehaviour = collider.GetComponent<EntityBehaviour>();
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

        private bool IsProduceMachine(GameEntity entity)
        {
            return entity.hasWorkbench || entity.hasProduceMachine || entity.hasAnvil;
        }
    }
}