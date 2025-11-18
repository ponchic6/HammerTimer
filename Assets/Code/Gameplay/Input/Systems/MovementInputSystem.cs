using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input.Systems
{
    public class MovementInputSystem : IExecuteSystem
    {
        private readonly IInputService _inputService;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public MovementInputSystem(IInputService inputService)
        {
            _inputService = inputService;
            
            Contexts contexts = Contexts.sharedInstance;
            _game = contexts.game;
            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.MovementInput));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                Vector2 inputDirection = _inputService.GetMoveDirection();
                inputDirection = inputDirection.normalized;
                entity.ReplaceMovementInput(inputDirection);
            }
        }
    }
}