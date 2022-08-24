using Assets.Scripts.Services.App;
using Assets.Scripts.Services.Game;
using Assets.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Global.Installers
{
    public class Projectinstaller : MonoInstaller, IInitializable
    {
        [SerializeField] private PlayerInventoryService playerInventoryService = null;
        [SerializeField] private PlayerSpecsService playerSpecsService = null;
        [SerializeField] private GenBlockSpawnerService genBlockSpawnerService;

        public UIManager UIManager { get; set; }

        public override void InstallBindings()
        {
            BindPlayerInventoryService();
            BindPlayerSpecsService();
            BindGenBlockSpawnerService();

            BindInstallerInterfaces();
        }

        public void Initialize()
        {
            playerInventoryService.Initialize();
        }

        private void BindGenBlockSpawnerService()
        {
            Container.Bind<GenBlockSpawnerService>().FromInstance(genBlockSpawnerService).AsSingle();
        }

        private void BindPlayerInventoryService()
        {
            Container.Bind<PlayerInventoryService>().FromInstance(playerInventoryService).AsSingle();
        }

        private void BindPlayerSpecsService()
        {
            Container.Bind<PlayerSpecsService>().FromInstance(playerSpecsService).AsSingle();
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
