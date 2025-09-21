using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input.Systems
{
    public class InitializeInputSystem : IInitializeSystem
    {
        private readonly IIdentifierService _identifierService;
        private readonly GameContext _game;

        public InitializeInputSystem(IIdentifierService identifierService)
        {
            _identifierService = identifierService;
            
            Contexts contexts = Contexts.sharedInstance;
            _game = contexts.game;
        }
        
        public void Initialize()
        {
            GameEntity inputEntity = _game.CreateEntity();
            inputEntity.AddId(_identifierService.Next());
            inputEntity.AddMovementInput(Vector2.zero);
        }
    }
}