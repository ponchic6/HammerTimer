using Entitas;

namespace Code.Gameplay.Player.Systems
{
    public class PlayerAnimationSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _players;
        private readonly IGroup<GameEntity> _inputs;
        private readonly GameContext _game;

        public PlayerAnimationSystem()
        {
            _game = Contexts.sharedInstance.game;

            _players = _game.GetGroup(GameMatcher.PlayerAnimator);
            _inputs = _game.GetGroup(GameMatcher.MovementInput);
        }

        public void Execute()
        {
            foreach (GameEntity input in _inputs)
            foreach (GameEntity player in _players)
            {
                if (input.movementInput.Value.sqrMagnitude >= 0.01f)
                {
                    player.playerAnimator.Value.TrySetRunState();
                }
                else
                {
                    player.playerAnimator.Value.TrySetIdleState();
                }
            }
        }
    }
}