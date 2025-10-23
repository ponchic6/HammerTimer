using System.Collections.Generic;
using Code.Gameplay.Produce.StaticData;
using Code.Infrastructure.StaticData;
using Entitas;

namespace Code.Gameplay.Produce.Systems
{
    public class WorkbenchRecipeValidateSystem : IExecuteSystem
    {
        private readonly CommonStaticData _commonStaticData;
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;

        public WorkbenchRecipeValidateSystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Workbench, GameMatcher.ProducingByPlayer));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities)
            {
                if (!entity.hasProduceProgress)
                {
                    List<string> currentIngredients = entity.workbench.Value;
                
                    foreach (WorkbenchRecipe recipe in _commonStaticData.workbenchRecipes)
                    {
                        if (HasMatchingIngredients(currentIngredients, recipe.from))
                        {
                            entity.AddProduceProgress(0f, recipe.to);
                        }
                    }
                }
            }
        }

        private bool HasMatchingIngredients(List<string> currentIngredients, List<string> recipeIngredients)
        {
            if (currentIngredients.Count != recipeIngredients.Count)
                return false;

            List<string> current = new List<string>(currentIngredients);
            List<string> required = new List<string>(recipeIngredients);

            current.Sort();
            required.Sort();

            for (int i = 0; i < current.Count; i++)
            {
                if (current[i] != required[i])
                    return false;
            }

            return true;
        }
    }
}