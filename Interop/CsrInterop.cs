using ConnectionSettingsRando;

namespace LegacyRando {
    internal static class CsrInterop {
        public static void Hook() {
            CSR.Register(
                LegacyRando.instance.GetName(),
                () => LegacyRando.globalSettings,
                s => SettingsRandomizer.CopyTo(s, LegacyRando.globalSettings)
            );
        }
    }
}
