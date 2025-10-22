using System.Collections.Generic;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Gameplay.Grabbing.Services
{
    public class GrabbableFactory : IGrabbableFactory
    {
        private readonly GameContext _gameContext;
        private readonly IIdentifierService _identifierService;
        private readonly Dictionary<GrabbableEnum, string> _prefabPaths;

        public GrabbableFactory(IIdentifierService identifierService)
        {
            _gameContext = Contexts.sharedInstance.game;
            
            _identifierService = identifierService;
            _prefabPaths = new Dictionary<GrabbableEnum, string>
            {
                { GrabbableEnum.Iron, "GrabbableItems/Iron" },
                { GrabbableEnum.Pistol, "GrabbableItems/Pistol" }
            };
        }

        public void SpawnNearWithPlayer(GrabbableEnum grabbableType)
        {
            if (!_gameContext.isPlayer)
                return;

            GameEntity playerEntity = _gameContext.playerEntity;
            Vector3 spawnPosition = playerEntity.transform.Value.position + playerEntity.transform.Value.forward * 2f;
            
            CreateGrabbableEntity(grabbableType, spawnPosition);
        }

        private void CreateGrabbableEntity(GrabbableEnum grabbableType, Vector3 position)
        {
            GameEntity entity = _gameContext.CreateEntity();
            entity.AddId(_identifierService.Next());
            entity.AddViewPath(_prefabPaths[grabbableType]);
            entity.AddInitialTransform(position, Quaternion.identity);
        }
    }
}