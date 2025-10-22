using Code.Gameplay;
using Code.Infrastructure.Systems;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class EcsRunner : MonoBehaviour
    {
        private ISystemFactory _systemFactory;
        private MainFeature _mainFeature;

        [Inject]
        public void Construct(ISystemFactory systemFactory)
        {
            _systemFactory = systemFactory;
        }

        private void Start()
        {
            _mainFeature = _systemFactory.Create<MainFeature>();
            _mainFeature.Initialize();
        }

        private void Update()
        {
            _mainFeature.Execute();
            _mainFeature.Cleanup();
        }
        
        private void OnDestroy()
        {
            _mainFeature.TearDown();
        }
    }
}