using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using static RandomizerMod.Localization;

namespace LegacyRando {
    public class RandoMenuPage {
        internal MenuPage LegacyRandoPage;
        internal MenuElementFactory<GlobalSettings> legacyMEF;
        internal VerticalItemPanel legacyVIP;

        internal SmallButton JumpToLegacyButton;
        
        internal static RandoMenuPage Instance { get; private set; }

        public static void OnExitMenu() {
            Instance = null;
        }

        public static void Hook() {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button) {
            button = Instance.JumpToLegacyButton;
            return true;
        }

        private void SetTopLevelButtonColor() {
            if(JumpToLegacyButton != null)
                JumpToLegacyButton.Text.color = LegacyRando.globalSettings.Any ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }

        private static void ConstructMenu(MenuPage landingPage) => Instance = new(landingPage);

        private RandoMenuPage(MenuPage landingPage) {
            LegacyRandoPage = new MenuPage(Localize("LegacyRando"), landingPage);
            legacyMEF = new(LegacyRandoPage, LegacyRando.globalSettings);
            legacyVIP = new(LegacyRandoPage, new(0, 300), 75f, true, legacyMEF.Elements);
            Localize(legacyMEF);
            foreach(IValueElement e in legacyMEF.Elements)
                e.SelfChanged += obj => SetTopLevelButtonColor();

            JumpToLegacyButton = new(landingPage, Localize("LegacyRando"));
            JumpToLegacyButton.AddHideAndShowEvent(landingPage, LegacyRandoPage);
            SetTopLevelButtonColor();
        }

        internal void ResetMenu(GlobalSettings settings) {
            legacyMEF.SetMenuValues(settings);
            SetTopLevelButtonColor();
        }
    }
}
