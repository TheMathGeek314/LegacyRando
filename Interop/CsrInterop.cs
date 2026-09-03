using ConnectionSettingsRando;

namespace MiscRando {
    internal static class CsrInterop {
        public static void Hook() {
            CSR.Register(
                MiscRando.instance.GetName(),
                () => MiscRando.globalSettings,
                s => SettingsRandomizer.CopyTo(s, MiscRando.globalSettings)
            );
        }
    }
}
