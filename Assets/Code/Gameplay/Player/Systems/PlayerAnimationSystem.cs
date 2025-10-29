using Code.Infrastructure.StaticData;
using Entitas;

namespace Code.Gameplay.Player.Systems
{
    public class PlayerAnimationSystem : IExecuteSystem
    {
        private readonly CommonStaticData _commonStaticData;
        private readonly IGroup<GameEntity> _players;
        private readonly GameContext _game;

        public PlayerAnimationSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _players = _game.GetGroup(GameMatcher.PlayerAnimator);
        }

        public void Execute()
        {
            foreach (GameEntity player in _players)
            {
                float currentSpeed = _game.playerEntity.currentSpeed.Value;
                float maxSpeed = _commonStaticData.maxPlayerSpeed;
                player.playerAnimator.Value.SetBlendMotion(currentSpeed / maxSpeed);
            }
        }
    }
}