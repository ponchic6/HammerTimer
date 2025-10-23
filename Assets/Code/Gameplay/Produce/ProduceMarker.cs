using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Produce
{
    public class ProduceMarker : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
        private Collider[] _overlapResults = new Collider[4];
        private GameContext _game;

        [Inject]
        public void Construct()
        {
            _game = Contexts.sharedInstance.game;
        }

        private void Update()
        {
            if (_game.inputEntity.isInteractDownInput && !_game.isProducingByPlayer && _game.inputEntity.isHoldingInteractInput)
            {
                Vector3 position = transform.position + transform.forward;
                int socketCount = Physics.OverlapSphereNonAlloc(position, _interactionRadius, _overlapResults, _socketLayer);
                GameEntity socketEntity = socketCount > 0 ? _overlapResults[0].GetComponent<EntityBehaviour>().Entity : null;
            
                if (socketEntity != null && (socketEntity.hasWorkbench || socketEntity.hasProduceMachine))
                    socketEntity.isProducingByPlayer = true;
                
                return;
            }
            
            if (!_game.inputEntity.isHoldingInteractInput)
            {
                if (_game.isProducingByPlayer)
                {
                    _game.producingByPlayerEntity.isProducingByPlayer = false;
                }
                return;
            }

            if (_game.isProducingByPlayer)
            {
                Vector3 position = transform.position + transform.forward;
                int socketCount = Physics.OverlapSphereNonAlloc(position, _interactionRadius, _overlapResults, _socketLayer);
                GameEntity socketEntity = socketCount > 0 ? _overlapResults[0].GetComponent<EntityBehaviour>().Entity : null;

                if (socketEntity == null || (!socketEntity.hasWorkbench && !socketEntity.hasProduceMachine)) 
                    _game.producingByPlayerEntity.isProducingByPlayer = false;
            }
        }
    }
}