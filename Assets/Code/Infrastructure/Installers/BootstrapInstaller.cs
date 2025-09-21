using Code.Infrastructure.Services;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private CommonStaticData _commonStaticData;
        
        public override void InstallBindings()
        {
            Container.Bind<IContext<GameEntity>>().FromInstance(Contexts.sharedInstance.game).AsSingle();
            Container.Bind<CommonStaticData>().FromInstance(_commonStaticData).AsSingle();
            Container.Bind<IIdentifierService>().To<IdentifierService>().AsSingle();
            Container.Bind<ISystemFactory>().To<SystemFactory>().AsSingle();
            Container.Bind<IEntityViewFactory>().To<EntityViewFactory>().AsSingle();
            
            UnityInputService unityInputService = new UnityInputService();
            Container.Bind<IInputService>().FromInstance(unityInputService);
            Container.Bind<IReadOnlyInputService>().FromInstance(unityInputService);
        }
    }
}