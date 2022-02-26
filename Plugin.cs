using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using SiraUtil.Zenject;
using SliceVisualizer.Configuration;
using SliceVisualizer.Installers;


namespace SliceVisualizer
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Logger Log = null!;
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        [Init]
        public void Init(Logger logger, Config conf, Zenjector zenject)
        {
            Log = logger;
            zenject.UseLogger(logger);
            zenject.Install<NsvAppInstaller>(Location.App, conf.Generated<PluginConfig>());
            zenject.Install<NsvGameInstaller>(Location.GameCore);
        }

        [OnEnable, OnDisable]
        public void OnStateChanged()
        {
            // Zenject is poggers
        }
    }
}
