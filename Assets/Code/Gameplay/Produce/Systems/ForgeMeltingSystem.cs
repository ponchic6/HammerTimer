using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Systems
{
    public class ForgeMeltingSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly GameContext _game;

        public ForgeMeltingSystem()
        {
            _game = Contexts.sharedInstance.game;
            
            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Forge, GameMatcher.ProduceProgress));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                float delta = Time.deltaTime * 0.3f;
                float progress = entity.produceProgress.Progress;

                if (entity.forge.Coal > 0)
                    entity.produceProgress.Progress = Mathf.Min(progress + delta, 1f);
                else if (progress > 0f)
                    entity.produceProgress.Progress = Mathf.Max(progress - delta, 0f);
            }
        }
    }
}