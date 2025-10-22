using UnityEngine;

namespace Code.Gameplay.Grabbing.Services
{
    public interface IGrabbableFactory
    {
        public void SpawnNearWithPlayer(GrabbableEnum grabbableType);
    }
}
