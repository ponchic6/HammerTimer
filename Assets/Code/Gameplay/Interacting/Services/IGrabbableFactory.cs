using Code.Gameplay.Produce;
using Code.Gameplay.Produce.Moulding;
using Code.Gameplay.Produce.View;
using UnityEngine;

namespace Code.Gameplay.Interacting.Services
{
    public interface IGrabbableFactory
    {
        public void SpawnViewNearWithPlayer(ItemsEnum grabbableEnum);
        public GameEntity SpawnAtPosition(ItemsEnum grabbableEnum, Vector3 position, bool active = true, MoldEnum? moldEnum = null);
    }
}
