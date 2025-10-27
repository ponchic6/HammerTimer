using Code.Gameplay.Grabbing.Services;
using Code.Infrastructure.View;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Grabbing.Grabbers
{
    public class InfinityBoxInteractor : MonoBehaviour
    {
        private IGrabbableFactory _grabbableFactory;

        [Inject]
        public void Construct(IGrabbableFactory grabbableFactory)
        {
            _grabbableFactory = grabbableFactory;
        }
        
        public bool TryGrabItem(EntityBehaviour playerEntityBehavior, GameEntity socketEntity)
        {
            if (!socketEntity.hasInfinityBox)
                return false;

            GameEntity grabbableEntity = _grabbableFactory.SpawnAtPosition(socketEntity.infinityBox.Value, transform.position, false);
            playerEntityBehavior.Entity.AddGrabbedItem(grabbableEntity.id.Value);

            return true;
        }
    }
}