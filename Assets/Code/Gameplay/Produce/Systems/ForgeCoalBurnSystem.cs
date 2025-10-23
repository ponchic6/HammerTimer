using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Systems
{
    public class ForgeCoalBurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public ForgeCoalBurnSystem()
        {
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.Forge);
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if (entity.forge.Coal > 0)
                    entity.forge.Coal -= Time.deltaTime;
            }
        }
    }
}