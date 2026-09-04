using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace LegacyRando {
    internal static class RsmInterop {
        public static void Hook() {
            RandoSettingsManagerMod.Instance.RegisterConnection(new LegacySettingsProxy());
        }
    }

    internal class LegacySettingsProxy: RandoSettingsProxy<GlobalSettings, string> {
        public override string ModKey => LegacyRando.instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; } = new EqualityVersioningPolicy<string>(LegacyRando.instance.GetVersion());

        public override void ReceiveSettings(GlobalSettings settings) {
            settings ??= new();
            RandoMenuPage.Instance.ResetMenu(settings);
        }

        public override bool TryProvideSettings(out GlobalSettings settings) {
            settings = LegacyRando.globalSettings;
            return settings.Any;
        }
    }
}
