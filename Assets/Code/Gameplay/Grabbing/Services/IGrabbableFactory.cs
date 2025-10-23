using UnityEngine;

namespace Code.Gameplay.Grabbing.Services
{
    public interface IGrabbableFactory
    {
        public void SpawnNearWithPlayer(string grabbableId);
        public void SpawnAtPosition(string grabbableId, Vector3 position);
    }
}
