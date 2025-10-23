using System;
using System.Collections.Generic;
using Code.Infrastructure.Services;
using Code.Infrastructure.StaticData;
using Entitas;

namespace Code.Gameplay.Input.Systems
{
    public class InteractInputSystem : IExecuteSystem
    {
        private readonly IReadOnlyInputService _inputService;
        private readonly CommonStaticData _staticData;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _inputEntities;
        private readonly IGroup<GameEntity> _singleInteractEntities;
        private readonly IGroup<GameEntity> _doubleInteractEntities;
        private readonly IGroup<GameEntity> _interactTimeEntities;
        private List<GameEntity> _buffer1 = new(4);
        private List<GameEntity> _buffer2 = new(4);
        private List<GameEntity> _buffer3 = new(4);

        public InteractInputSystem(IReadOnlyInputService inputService, CommonStaticData staticData)
        {
            _inputService = inputService;
            _staticData = staticData;
            _game = Contexts.sharedInstance.game;
            _inputEntities = _game.GetGroup(GameMatcher.Input);
            _singleInteractEntities = _game.GetGroup(GameMatcher.InteractDownInput);
            _doubleInteractEntities = _game.GetGroup(GameMatcher.DoubleInteractDownInput);
            _interactTimeEntities = _game.GetGroup(GameMatcher.TimeOfInteractDownInput);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _singleInteractEntities.GetEntities(_buffer1))
                entity.isInteractDownInput = false;
            
            foreach (GameEntity entity in _doubleInteractEntities.GetEntities(_buffer2))
                entity.isDoubleInteractDownInput = false;
            
            foreach (GameEntity entity in _inputEntities)
            {
                entity.isHoldingInteractInput = _inputService.GetInteractKey();
                
                if (_inputService.GetInteractKeyDown())
                {
                    if (entity.hasTimeOfInteractDownInput)
                    {
                        TimeSpan timeSinceLastClick = DateTime.Now.TimeOfDay - entity.timeOfInteractDownInput.TimeAdded;

                        if (timeSinceLastClick.TotalSeconds < _staticData.doubleClickThreshold)
                        {
                            entity.isDoubleInteractDownInput = true;
                            entity.RemoveTimeOfInteractDownInput();
                            continue;
                        }
                    }

                    entity.ReplaceTimeOfInteractDownInput(DateTime.Now.TimeOfDay);
                }
                
                else
                {
                    foreach (GameEntity interactTimeEntity in _interactTimeEntities.GetEntities(_buffer3))
                    {
                        TimeSpan timeSinceClick = DateTime.Now.TimeOfDay - interactTimeEntity.timeOfInteractDownInput.TimeAdded;

                        if (timeSinceClick.TotalSeconds >= _staticData.doubleClickThreshold)
                        {
                            entity.RemoveTimeOfInteractDownInput();
                            entity.isInteractDownInput = true;
                        }
                    }
                }
            }
        }
    }
}
