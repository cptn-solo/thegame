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

        [SerializeField] private ApplicationService applicationService = null;
        [SerializeField] private PlayerPreferencesService playerPreferencesService = null;
        [SerializeField] private ArtefactSpawnerService artefactSpawnerService;

        [SerializeField] private AudioPlaybackService audioPlaybackService = null;


        public UIManager UIManager { get; set; }

        public override void InstallBindings()
        {
            BindApplicationService();
            BindPlayerPreferencesService();
            BindAudioPlaybackService();
            BindArtefactSpawnerService();

            BindPlayerInventoryService();
            BindPlayerSpecsService();
            BindGenBlockSpawnerService();

            BindInstallerInterfaces();
        }

        public void Initialize()
        {
            playerPreferencesService.InitPlayerPreferences();
            playerSpecsService.InitPlayerSpecs();
            audioPlaybackService.InitAudioPlayback();

            playerInventoryService.Initialize();
        }

        private void BindAudioPlaybackService()
        {
            Container.Bind<AudioPlaybackService>().FromInstance(audioPlaybackService).AsSingle();
        }
        private void BindPlayerPreferencesService()
        {
            Container.Bind<PlayerPreferencesService>().FromInstance(playerPreferencesService).AsSingle();
        }
        private void BindApplicationService()
        {
            Container.Bind<ApplicationService>().FromInstance(applicationService).AsSingle();
        }
        private void BindArtefactSpawnerService()
        {
            Container.Bind<ArtefactSpawnerService>().FromInstance(artefactSpawnerService).AsSingle();
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
