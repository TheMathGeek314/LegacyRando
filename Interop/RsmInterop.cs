using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace MiscRando {
    internal static class RsmInterop {
        public static void Hook() {
            RandoSettingsManagerMod.Instance.RegisterConnection(new MiscSettingsProxy());
        }
    }

    internal class MiscSettingsProxy: RandoSettingsProxy<GlobalSettings, string> {
        public override string ModKey => MiscRando.instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; } = new EqualityVersioningPolicy<string>(MiscRando.instance.GetVersion());

        public override void ReceiveSettings(GlobalSettings settings) {
            settings ??= new();
            RandoMenuPage.Instance.ResetMenu(settings);
        }

        public override bool TryProvideSettings(out GlobalSettings settings) {
            settings = MiscRando.globalSettings;
            return settings.Any;
        }
    }
}
