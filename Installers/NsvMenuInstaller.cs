using SliceVisualizer.UI;
using Zenject;

namespace SliceVisualizer.Installers
{
    internal class NsvMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SettingsUI>().AsSingle().NonLazy();
        }
    }
}