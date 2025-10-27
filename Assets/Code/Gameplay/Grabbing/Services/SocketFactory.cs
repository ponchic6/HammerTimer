using Code.Infrastructure.Services;
using Code.Infrastructure.StaticData;
using UnityEngine;

namespace Code.Gameplay.Grabbing.Services
{
    public class SocketFactory : ISocketFactory
    {
        private readonly CommonStaticData _commonStaticData;
        private readonly IIdentifierService _identifierService;
        private readonly GameContext _game;

        public SocketFactory(CommonStaticData commonStaticData, IIdentifierService identifierService)
        {
            _commonStaticData = commonStaticData;
            _identifierService = identifierService;
            _game = Contexts.sharedInstance.game;
        }
        
        public void SpawnShelfSocket(Vector3 position)
        {
            GameEntity shelfEntity = _game.CreateEntity();
            shelfEntity.AddId(_identifierService.Next());
            shelfEntity.AddViewPrefab(_commonStaticData.shelf);
            shelfEntity.AddInitialTransform(position, Quaternion.identity);
        }

        public void SpawnProduceMachine(Vector3 position)
        {
            GameEntity shelfEntity = _game.CreateEntity();
            shelfEntity.AddId(_identifierService.Next());
            shelfEntity.AddProduceMachine(ItemsEnum.IronIngot, ItemsEnum.IronIngot);
            shelfEntity.AddViewPrefab(_commonStaticData.produceMachine);
            shelfEntity.AddInitialTransform(position, Quaternion.identity);
        }
    }
}