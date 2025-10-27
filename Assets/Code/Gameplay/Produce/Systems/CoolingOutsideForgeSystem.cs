using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Systems
{
    public class CoolingOutsideForgeSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _grabbables;
        private readonly IGroup<GameEntity> _forges;
        private readonly CommonStaticData _commonStaticData;

        public CoolingOutsideForgeSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _grabbables = _game.GetGroup(GameMatcher.GrabbableTemperature);
            _forges = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Forge, GameMatcher.GrabbedItem));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _grabbables)
            {
                if (entity.grabbableTemperature.Value <= _commonStaticData.environmentTemperature)
                    continue;

                bool isInForge = false;

                foreach (GameEntity forge in _forges)
                {
                    if (forge.grabbedItem.Value == entity.id.Value)
                    {
                        isInForge = true;
                        break;
                    }
                }

                if (!isInForge) 
                    entity.grabbableTemperature.Value = Mathf.Max(_commonStaticData.environmentTemperature, entity.grabbableTemperature.Value - _commonStaticData.itemCoolingRate * Time.deltaTime);
            }
        }
    }
}