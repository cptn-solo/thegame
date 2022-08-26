using Assets.Scripts.Services.App;
using Assets.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Global.Installers
{

    public class MainSceneInstaller : MonoInstaller, IInitializable
    {
        [Inject] private readonly PlayerSpecsService playerSpecsService;

        #region SerializeFields

        [SerializeField] private UIManager uiManager = null;

        [SerializeField] private MainSceneCamera mainCamera = null;

        #endregion

        #region Public
        public override void InstallBindings()
        {
            BindMainSceneCamera();

            BindUIManager();

            BindInstallerInterfaces();
        }

        public void Initialize()
        {
            playerSpecsService.HUDScreen = uiManager.HUDScreen;
        }
        #endregion

        #region Private methods
        private void BindMainSceneCamera()
        {
            Container.Bind<MainSceneCamera>().FromInstance(mainCamera).AsSingle();
        }

        private void BindUIManager()
        {
            Container.Bind<UIManager>().FromInstance(uiManager).AsSingle();
        }

        private void BindInstallerInterfaces()
        {
            Container
                .BindInterfacesTo<MainSceneInstaller>()
                .FromInstance(this)
                .AsSingle();
        }
        #endregion
    }
}
