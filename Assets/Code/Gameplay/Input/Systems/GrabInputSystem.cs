using Code.Infrastructure.Services;
using Entitas;

namespace Code.Gameplay.Input.Systems
{
    public class GrabInputSystem : IExecuteSystem
    {
        private readonly IReadOnlyInputService _inputService;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public GrabInputSystem(IReadOnlyInputService inputService)
        {
            _inputService = inputService;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.Input);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                bool interactKeyDown = _inputService.GetInteractKeyDown();
                entity.isInteractInput = interactKeyDown;
            }
        }
    }
}