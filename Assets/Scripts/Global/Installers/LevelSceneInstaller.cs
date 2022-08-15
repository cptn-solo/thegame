using Zenject;

namespace Assets.Scripts.Global.Installers
{
    public class LevelSceneInstaller : MonoInstaller, IInitializable
    {
        #region SerializeFields
        #endregion

        #region Private
        #endregion

        #region Public methods
        public override void InstallBindings()
        {
            BindInstallerInterfaces();
        }

        public void Initialize()
        {
        }
        #endregion

        #region Private methods
        private void BindInstallerInterfaces()
        {
            Container
                .BindInterfacesTo<LevelSceneInstaller>()
                .FromInstance(this)
                .AsSingle();
        }
        #endregion
    }
}
