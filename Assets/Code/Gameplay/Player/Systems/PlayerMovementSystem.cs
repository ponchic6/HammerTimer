using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Systems
{
    public class PlayerMovementSystem : IExecuteSystem
    {
        private readonly GameContext _gameContext;
        private readonly IGroup<GameEntity> _players;
        private readonly IGroup<GameEntity> _inputs;

        public PlayerMovementSystem()
        {
            Contexts contexts = Contexts.sharedInstance;
            _gameContext = contexts.game;
            _players = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Player, GameMatcher.Transform, GameMatcher.Speed));
            _inputs = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.MovementInput));
            
        }

        public void Execute()
        {
            foreach (GameEntity input in _inputs)
            foreach (GameEntity player in _players)
            {
                float deltaTime = Time.deltaTime;
                Vector2 direction = input.movementInput.direction;
                float speed = player.speed.Value;
                Vector3 delta = new Vector2(direction.x, direction.y) * speed * deltaTime;
                player.transform.Value.position += delta;
            }
        }
    }
}
