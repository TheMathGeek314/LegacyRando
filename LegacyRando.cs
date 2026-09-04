using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyRando {
    public class LegacyRando: Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings> {
        new public string GetName() => "LegacyRando";
        public override string GetVersion() => "1.0.0.0";

        public static GlobalSettings globalSettings { get; set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;

        public static LocalSettings localSettings { get; set; } = new();
        public void OnLoadLocal(LocalSettings s) => localSettings = s;
        public LocalSettings OnSaveLocal() => localSettings;

        internal static LegacyRando instance;

        public LegacyRando(): base(null) {
            instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            LegacyModule.totemPrefab = preloadedObjects["Fungus1_30"]["Soul Totem mini_horned"];
            LegacyModule.totemBasePrefab = preloadedObjects["Fungus1_30"]["Mini_totems_0000_7"];
            LegacyModule.tollGatePrefab = preloadedObjects["Mines_33"]["Toll Gate"];
            LegacyModule.tollMachinePrefab = preloadedObjects["Mines_33"]["Toll Gate Machine"];
            LegacyModule.bellPrefab = preloadedObjects["Crossroads_47"]["_Scenery/Station Bell"];

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += LegacyModule.EarlyHook;

            RandoInterop.Hook();
        }

        public override List<(string, string)> GetPreloadNames() {
            return [
                ("Crossroads_47", "_Scenery/Station Bell"),
                ("Fungus1_30", "Soul Totem mini_horned"),
                ("Fungus1_30", "Mini_totems_0000_7"),
                ("Mines_33", "Toll Gate Machine"),
                ("Mines_33", "Toll Gate")
            ];
        }
    }
}