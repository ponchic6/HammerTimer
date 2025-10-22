using Code.Infrastructure.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player
{
    public class PlayerProduceMarker : MonoBehaviour
    {
        [SerializeField] private float _interactionRadius = 0.5f;
        [SerializeField] private LayerMask _socketLayer;
        private Collider[] _overlapResults = new Collider[4];
        private IReadOnlyInputService _inputService;
        private GameContext _gameContext;

        [Inject]
        public void Construct(IReadOnlyInputService inputService)
        {
            _inputService = inputService;
            _gameContext = Contexts.sharedInstance.game;
        }
        
        private void Update()
        {
            if (!_inputService.GetInteractKey())
            {
                if (_gameContext.isProducingByPlayer) 
                    _gameContext.producingByPlayerEntity.isProducingByPlayer = false;
                return;
            }
            
            Vector3 position = transform.position + transform.forward;
            int socketCount = Physics.OverlapSphereNonAlloc(position, _interactionRadius, _overlapResults, _socketLayer);
            GameEntity socketEntity = socketCount > 0 ? _overlapResults[0].GetComponent<EntityBehaviour>().Entity : null;
            
            if (socketEntity is { hasGrabbedItem: true, hasProduceProgress: true })
            {
                socketEntity.isProducingByPlayer = true;
            }
            else
            {
                if (_gameContext.isProducingByPlayer) 
                    _gameContext.producingByPlayerEntity.isProducingByPlayer = false;
            }
        }
    }
}