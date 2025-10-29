using Code.Gameplay.Produce;
using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.View;
using Code.Infrastructure.Services;
using Code.Infrastructure.StaticData;
using UnityEngine;

namespace Code.Gameplay.Interacting.Services
{
    public class GrabbableFactory : IGrabbableFactory
    {
        private readonly GameContext _gameContext;
        private readonly IIdentifierService _identifierService;
        private readonly CommonStaticData _commonStaticData;

        public GrabbableFactory(IIdentifierService identifierService, CommonStaticData commonStaticData)
        {
            _gameContext = Contexts.sharedInstance.game;
            
            _identifierService = identifierService;
            _commonStaticData = commonStaticData;
        }

        public void SpawnViewNearWithPlayer(ItemsEnum grabbableEnum)
        {
            if (!_gameContext.isPlayer)
                return;

            GameEntity playerEntity = _gameContext.playerEntity;
            Vector3 spawnPosition = playerEntity.transform.Value.position + playerEntity.transform.Value.forward * 2f;

            SpawnAtPosition(grabbableEnum, spawnPosition);
        }

        public GameEntity SpawnAtPosition(ItemsEnum grabbableEnum, Vector3 position, bool active = true, MoldEnum? moldEnum = null)
        {
            GameEntity entity = _gameContext.CreateEntity();
            entity.AddId(_identifierService.Next());
            entity.AddViewPath("GrabbableItem");
            entity.AddGrabbableItem(grabbableEnum);
            entity.AddInitialTransform(position, Quaternion.identity);
            entity.AddInitialViewState(active);
            
            if (moldEnum.HasValue)
                entity.AddMold(moldEnum.Value);
            
            return entity;
        }
    }
}