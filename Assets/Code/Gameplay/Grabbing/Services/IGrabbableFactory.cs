using Code.Gameplay.Produce;
using UnityEngine;

namespace Code.Gameplay.Grabbing.Services
{
    public interface IGrabbableFactory
    {
        public void SpawnViewNearWithPlayer(ItemsEnum grabbableEnum);
        public GameEntity SpawnAtPosition(ItemsEnum grabbableEnum, Vector3 position, bool active = true, MoldEnum? moldEnum = null);
    }
}
