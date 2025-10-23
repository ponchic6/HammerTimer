using System.Collections.Generic;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Gameplay.Grabbing.Services
{
    public class GrabbableFactory : IGrabbableFactory
    {
        private readonly GameContext _gameContext;
        private readonly IIdentifierService _identifierService;

        public GrabbableFactory(IIdentifierService identifierService)
        {
            _gameContext = Contexts.sharedInstance.game;
            
            _identifierService = identifierService;
        }

        public void SpawnNearWithPlayer(string grabbableId)
        {
            if (!_gameContext.isPlayer)
                return;

            GameEntity playerEntity = _gameContext.playerEntity;
            Vector3 spawnPosition = playerEntity.transform.Value.position + playerEntity.transform.Value.forward * 2f;

            CreateGrabbableEntity(grabbableId, spawnPosition);
        }

        public void SpawnAtPosition(string grabbableId, Vector3 position)
        {
            CreateGrabbableEntity(grabbableId, position);
        }

        private void CreateGrabbableEntity(string grabbableId, Vector3 position)
        {
            GameEntity entity = _gameContext.CreateEntity();
            entity.AddId(_identifierService.Next());
            entity.AddViewPath(grabbableId);
            entity.AddGrabbableItem(grabbableId);
            entity.AddInitialTransform(position, Quaternion.identity);
        }
    }
}