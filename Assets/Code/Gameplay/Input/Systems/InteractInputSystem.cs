using System.Collections.Generic;
using Code.Infrastructure.Services;
using Entitas;

namespace Code.Gameplay.Input.Systems
{
    public class InteractInputSystem : IExecuteSystem
    {
        private readonly IReadOnlyInputService _inputService;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _inputEntities;
        private readonly IGroup<GameEntity> _singleInteractEntities;
        private List<GameEntity> _buffer = new(4);

        public InteractInputSystem(IReadOnlyInputService inputService)
        {
            _inputService = inputService;
            _game = Contexts.sharedInstance.game;
            _inputEntities = _game.GetGroup(GameMatcher.Input);
            _singleInteractEntities = _game.GetGroup(GameMatcher.InteractDownInput);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _singleInteractEntities.GetEntities(_buffer))
                entity.isInteractDownInput = false;

            foreach (GameEntity entity in _inputEntities)
            {
                entity.isHoldingInteractInput = _inputService.GetInteractKey();

                if (_inputService.GetInteractKeyDown())
                {
                    entity.isInteractDownInput = true;
                }
            }
        }
    }
}
