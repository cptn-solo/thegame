using Assets.Scripts.Services.App;
using Assets.Scripts.Services.Scenes;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Global.Installers
{
    public class MainSceneInstaller : MonoInstaller
    {
        #region SerializeFields
        [SerializeField] private ApplicationService applicationService = null;
        [SerializeField] private MainSceneCamera mainCamera = null;
        #endregion

        #region Public
        public override void InstallBindings()
        {
            BindApplicationService();
            BindMainSceneCamera();

        }

        #endregion

        #region Private methods
        private void BindMainSceneCamera()
        {
            Container.Bind<MainSceneCamera>().FromInstance(mainCamera).AsSingle();
        }

        private void BindApplicationService()
        {
            Container.Bind<ApplicationService>().FromInstance(applicationService).AsSingle();
        }
        #endregion
    }
}
