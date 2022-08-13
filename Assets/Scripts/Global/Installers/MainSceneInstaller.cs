using Assets.Scripts.Services.App;
using Assets.Scripts.Services.Game;
using Assets.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Global.Installers
{

    public class MainSceneInstaller : MonoInstaller, IInitializable
    {
        #region SerializeFields
        [SerializeField] private ApplicationService applicationService = null;
        [SerializeField] private PlayerPreferencesService playerPreferencesService = null;
        [SerializeField] private ArtefactSpawnerService artefactSpawnerService;

        [SerializeField] private AudioPlaybackService audioPlaybackService = null;

        [SerializeField] private UIManager uiManager = null;

        [SerializeField] private MainSceneCamera mainCamera = null;

        #endregion

        #region Public
        public override void InstallBindings()
        {
            BindApplicationService();
            BindPlayerPreferencesService();
            BindAudioPlaybackService();
            BindArtefactSpawnerService();

            BindMainSceneCamera();

            BindUIManager();

            BindInstallerInterfaces();
        }

        public void Initialize()
        {
            playerPreferencesService.InitPlayerPreferences();
            audioPlaybackService.InitAudioPlayback();
        }
        #endregion

        #region Private methods
        private void BindArtefactSpawnerService()
        {
            Container.Bind<ArtefactSpawnerService>().FromInstance(artefactSpawnerService).AsSingle();
        }

        private void BindMainSceneCamera()
        {
            Container.Bind<MainSceneCamera>().FromInstance(mainCamera).AsSingle();
        }

        private void BindAudioPlaybackService()
        {
            Container.Bind<AudioPlaybackService>().FromInstance(audioPlaybackService).AsSingle();
        }

        private void BindPlayerPreferencesService()
        {
            Container.Bind<PlayerPreferencesService>().FromInstance(playerPreferencesService).AsSingle();
        }

        private void BindUIManager()
        {
            Container.Bind<UIManager>().FromInstance(uiManager).AsSingle();
        }

        private void BindApplicationService()
        {
            Container.Bind<ApplicationService>().FromInstance(applicationService).AsSingle();
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
