using UnityEngine;

namespace Code.Gameplay.Interacting.Services
{
    public interface ISocketFactory
    {
        public void SpawnShelfSocket(Vector3 position);
        public void SpawnProduceMachine(Vector3 position);
    }
}