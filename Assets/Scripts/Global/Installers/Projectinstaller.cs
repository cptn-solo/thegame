using Assets.Scripts.Services.App;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Global.Installers
{
    public class Projectinstaller : MonoInstaller, IInitializable
    {
        [SerializeField] private PlayerInventoryService playerInventoryService = null;

        public override void InstallBindings()
        {
            BindPlayerInventoryService();
            BindInstallerInterfaces();

        }

        public void Initialize()
        {
            playerInventoryService.Initialize();
        }

        private void BindPlayerInventoryService()
        {
            Container.Bind<PlayerInventoryService>().FromInstance(playerInventoryService).AsSingle();
        }

        private void BindInstallerInterfaces()
        {
            Container
                .BindInterfacesTo<Projectinstaller>()
                .FromInstance(this)
                .AsSingle();
        }

    }
}
