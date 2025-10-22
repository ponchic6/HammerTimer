using UnityEngine;

namespace Code.Gameplay.Grabbing.Services
{
    public interface ISocketFactory
    {
        public void SpawnShelfSocket(Vector3 position);
        public void SpawnProduceMachine(Vector3 position);
    }
}