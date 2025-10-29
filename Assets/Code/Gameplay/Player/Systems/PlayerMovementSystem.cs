using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Systems
{
    public class PlayerMovementSystem : IExecuteSystem
    {
        private readonly GameContext _gameContext;
        private readonly IGroup<GameEntity> _players;
        private readonly IGroup<GameEntity> _inputs;
        private readonly CommonStaticData _commonStaticData;

        public PlayerMovementSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            Contexts contexts = Contexts.sharedInstance;
            _gameContext = contexts.game;
            _players = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Player, GameMatcher.Transform, GameMatcher.CurrentSpeed));
            _inputs = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.MovementInput));
        }

        public void Execute()
        {
            foreach (GameEntity input in _inputs)
            foreach (GameEntity player in _players)
            {
                Vector2 direction = input.movementInput.Value;
                float maxSpeed = _commonStaticData.maxPlayerSpeed;
                float currentSpeed = player.currentSpeed.Value;
                
                if (direction.sqrMagnitude > 0.01f)
                    currentSpeed = Mathf.Min(currentSpeed + _commonStaticData.acceleration * Time.deltaTime, maxSpeed);
                else
                    currentSpeed = Mathf.Max(currentSpeed - _commonStaticData.acceleration * Time.deltaTime, 0f);

                player.ReplaceCurrentSpeed(currentSpeed);

                Vector3 delta = new Vector3(direction.x, 0, direction.y).normalized * currentSpeed * Time.deltaTime;
                player.transform.Value.position += delta;

                if (direction.sqrMagnitude > 0.01f)
                {
                    Vector3 lookDirection = new Vector3(direction.x, 0, direction.y);
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    player.transform.Value.rotation = Quaternion.Slerp(player.transform.Value.rotation, targetRotation, 10f * Time.deltaTime);
                }
            }
        }
    }
}
