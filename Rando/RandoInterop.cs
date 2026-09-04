using Modding;
using System.IO;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace LegacyRando {
    internal static class RandoInterop {
        public static void Hook() {
            RandoMenuPage.Hook();
            RequestModifier.Hook();
            LogicAdder.Hook();

            DefineLocations();
            DefineItems();

            RandoController.OnExportCompleted += AddModules;
            SettingsLog.AfterLogSettings += LogRandoSettings;

            if(ModHooks.GetMod("ConnectionSettingsRando") is Mod)
                CsrInterop.Hook();

            if(ModHooks.GetMod("CondensedSpoilerLogger") is Mod)
                CondensedSpoilerLogger.AddCategory("Tolls", (args) => true, Consts.TollNames);

            if(ModHooks.GetMod("RandoSettingsManager") is Mod)
                RsmInterop.Hook();
        }

        private static void AddModules(RandoController controller) {
            if(!LegacyRando.globalSettings.Any)
                return;
            ItemChangerMod.Modules.GetOrAdd<LegacyModule>();
        }

        private static void LogRandoSettings(LogArguments args, TextWriter w) {
            w.WriteLine("Logging LegacyRando settings:");
            w.WriteLine(JsonUtil.Serialize(LegacyRando.globalSettings));
        }

        private static void DefineLocations() {
            static void DefLoc(AbstractLocation loc, ISprite sprite, float x, float y) {
                InteropTag tag = AddTag(loc);
                tag.Properties["PinSprite"] = sprite;
                tag.Properties["WorldMapLocation"] = (loc.sceneName, x, y);
                Finder.DefineCustomLocation(loc);
            }

            ObjectLocation totem = new() {
                objectName = "LegacyRando Totem",
                elevation = 0.2f,
                forceShiny = false,
                name = Consts.GreenpathTotem,
                sceneName = "Fungus1_03",
                flingType = FlingType.Everywhere
            };
            DefLoc(totem, new LegacySprite("TotemPin"), 54.04f, 12.627f);

            DefLoc(new TollLocation { name = Consts.PeakToll, sceneName = "Mines_33" }, new LegacySprite("TollPin"), 49, 14);
            DefLoc(new TollLocation { name = Consts.DeepnestToll, sceneName = "Deepnest_41" }, new LegacySprite("TollPin"), 99.8f, 88.12f);
        }

        private static void DefineItems() {
            TollItem pToll = new(Consts.PeakToll);
            Finder.DefineCustomItem(pToll);
            TollItem dToll = new(Consts.DeepnestToll);
            Finder.DefineCustomItem(dToll);
        }

        public static InteropTag AddTag(TaggableObject obj) {
            InteropTag tag = obj.GetOrAddTag<InteropTag>();
            tag.Message = "RandoSupplementalMetadata";
            tag.Properties["ModSource"] = LegacyRando.instance.GetName();
            return tag;
        }
    }
}
