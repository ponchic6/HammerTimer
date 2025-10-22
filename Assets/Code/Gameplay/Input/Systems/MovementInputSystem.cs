using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input.Systems
{
    public class MovementInputSystem : IExecuteSystem
    {
        private readonly IReadOnlyInputService _inputService;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public MovementInputSystem(IReadOnlyInputService inputService)
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
                Vector2 inputDirection = Vector2.zero;

                if (_inputService.GetKey(KeyCode.W))
                    inputDirection.y += 1f;
                if (_inputService.GetKey(KeyCode.S))
                    inputDirection.y -= 1f;
                if (_inputService.GetKey(KeyCode.A))
                    inputDirection.x -= 1f;
                if (_inputService.GetKey(KeyCode.D))
                    inputDirection.x += 1f;

                inputDirection = inputDirection.normalized;
                entity.ReplaceMovementInput(inputDirection);
            }
        }
    }
}