using System.IO;
using System.Reflection;
using RandomizerCore;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace MiscRando {
    public static class LogicAdder {
        public static void Hook() {
            RCData.RuntimeLogicOverride.Subscribe(50, ApplyLogic);
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            if(!MiscRando.globalSettings.Any)
                return;
            JsonLogicFormat fmt = new();
            Assembly a = typeof(LogicAdder).Assembly;
            string p = "MiscRando.Resources.";

            using Stream l = a.GetManifestResourceStream(p + "logic.json");
            lmb.DeserializeFile(LogicFileType.Locations, fmt, l);

            using Stream t = a.GetManifestResourceStream(p + "terms.json");
            lmb.DeserializeFile(LogicFileType.Terms, fmt, t);

            //hi nerthul

            if(MiscRando.globalSettings.StagNestBell) {
                //Can_Stag edits
            }
            if(MiscRando.globalSettings.PeakToll) {
                //geo rock, stalactite, glimback, BWR, transition edits
                //I have no idea whether these should be in a json or written here
                //Also whether connections need a different place than vanilla checks
            }
            if(MiscRando.globalSettings.DeepnestToll) {
                //good luck with midwife, corpse, BWR, deephunter, devout
            }

            lmb.AddItem(new SingleItem(Consts.PeakToll, new TermValue(lmb.GetTerm(Consts.PeakToll), 1)));
            lmb.AddItem(new SingleItem(Consts.DeepnestToll, new TermValue(lmb.GetTerm(Consts.DeepnestToll), 1)));
        }
    }
}
