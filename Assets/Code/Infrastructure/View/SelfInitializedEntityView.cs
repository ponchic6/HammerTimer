using Code.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.View
{
    public class SelfInitializedEntityView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour _entityBehaviour;
        private IIdentifierService _identifiers;

        [Inject]
        private void Construct(IIdentifierService identifiers) => 
            _identifiers = identifiers;

        private void Awake()
        {
            GameEntity entity = Contexts.sharedInstance.game.CreateEntity();
            entity.AddId(_identifiers.Next());
      
            _entityBehaviour.SetEntity(entity);
        }

    }
}