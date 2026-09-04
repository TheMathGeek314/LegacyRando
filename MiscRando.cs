using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace MiscRando {
    public class MiscRando: Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings> {
        new public string GetName() => "MiscRando";
        public override string GetVersion() => "1.0.0.0";

        public static GlobalSettings globalSettings { get; set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;

        public static LocalSettings localSettings { get; set; } = new();
        public void OnLoadLocal(LocalSettings s) => localSettings = s;
        public LocalSettings OnSaveLocal() => localSettings;

        internal static MiscRando instance;

        public MiscRando(): base(null) {
            instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            MiscModule.totemPrefab = preloadedObjects["Fungus1_30"]["Soul Totem mini_horned"];
            MiscModule.totemBasePrefab = preloadedObjects["Fungus1_30"]["Mini_totems_0000_7"];
            MiscModule.tollGatePrefab = preloadedObjects["Mines_33"]["Toll Gate"];
            MiscModule.tollMachinePrefab = preloadedObjects["Mines_33"]["Toll Gate Machine"];
            MiscModule.bellPrefab = preloadedObjects["Crossroads_47"]["_Scenery/Station Bell"];

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += MiscModule.EarlyHook;

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