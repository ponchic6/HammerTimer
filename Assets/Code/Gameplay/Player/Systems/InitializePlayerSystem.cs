using Code.Infrastructure.Services;
using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Systems
{
    public class InitializePlayerSystem : IInitializeSystem
    {
        private readonly IIdentifierService _identifierService;
        private readonly CommonStaticData _commonStaticData;
        private readonly GameContext _game;

        public InitializePlayerSystem(IIdentifierService identifierService, CommonStaticData commonStaticData)
        {
            _identifierService = identifierService;
            _commonStaticData = commonStaticData;
            
            Contexts contexts = Contexts.sharedInstance;
            _game = contexts.game;
        }

        public void Initialize()
        {
            GameEntity player = _game.CreateEntity();
            player.AddId(_identifierService.Next());
            player.AddSpeed(5f);
            player.isPlayer = true;
            player.AddViewPrefab(_commonStaticData.playerPrefab);

            Vector3 startPos = Vector3.zero;
            Quaternion startRot = Quaternion.identity;
            player.AddInitialTransform(startPos, startRot);
        }
    }
}
