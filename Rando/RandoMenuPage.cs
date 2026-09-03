using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using static RandomizerMod.Localization;

namespace MiscRando {
    public class RandoMenuPage {
        internal MenuPage MiscRandoPage;
        internal MenuElementFactory<GlobalSettings> miscMEF;
        internal VerticalItemPanel miscVIP;

        internal SmallButton JumpToMiscButton;
        
        internal static RandoMenuPage Instance { get; private set; }

        public static void OnExitMenu() {
            Instance = null;
        }

        public static void Hook() {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button) {
            button = Instance.JumpToMiscButton;
            return true;
        }

        private void SetTopLevelButtonColor() {
            if(JumpToMiscButton != null)
                JumpToMiscButton.Text.color = MiscRando.globalSettings.Any ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }

        private static void ConstructMenu(MenuPage landingPage) => Instance = new(landingPage);

        private RandoMenuPage(MenuPage landingPage) {
            MiscRandoPage = new MenuPage(Localize("MiscRando"), landingPage);
            miscMEF = new(MiscRandoPage, MiscRando.globalSettings);
            miscVIP = new(MiscRandoPage, new(0, 300), 75f, true, miscMEF.Elements);
            Localize(miscMEF);
            foreach(IValueElement e in miscMEF.Elements)
                e.SelfChanged += obj => SetTopLevelButtonColor();

            JumpToMiscButton = new(landingPage, Localize("MiscRando"));
            JumpToMiscButton.AddHideAndShowEvent(landingPage, MiscRandoPage);
            SetTopLevelButtonColor();
        }

        internal void ResetMenu(GlobalSettings settings) {
            miscMEF.SetMenuValues(settings);
            SetTopLevelButtonColor();
        }
    }
}
