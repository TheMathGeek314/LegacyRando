using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using RandomizerCore;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace LegacyRando {
    public static class LogicAdder {
        public static void Hook() {
            RCData.RuntimeLogicOverride.Subscribe(50f, ApplyLogic);
            RCData.RuntimeLogicOverride.Subscribe(100f, ConnectionPatch);
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            if(!LegacyRando.globalSettings.Any)
                return;
            JsonLogicFormat fmt = new();
            Assembly a = typeof(LogicAdder).Assembly;
            string p = "LegacyRando.Resources.";
            
            using Stream l = a.GetManifestResourceStream(p + "logic.json");
            lmb.DeserializeFile(LogicFileType.Locations, fmt, l);

            using Stream i = a.GetManifestResourceStream(p + "items.json");
            lmb.DeserializeFile(LogicFileType.ItemStrings, fmt, i);

            using Stream s = a.GetManifestResourceStream(p + "logicSubstitutions.json");
            lmb.DeserializeFile(LogicFileType.LogicSubst, fmt, s);

            using Stream t = a.GetManifestResourceStream(p + "terms.json");
            lmb.DeserializeFile(LogicFileType.Terms, fmt, t);

            // The only logic override happens to be Deepnest_41[right1], which we explicitly
            // want to exclude if this Toll isn't present. The rest of the changes are
            // handled by vanilla defs
            if(LegacyRando.globalSettings.DeepnestToll) {
                using Stream o = a.GetManifestResourceStream(p + "logicOverrides.json");
                lmb.DeserializeFile(LogicFileType.LogicEdit, fmt, o);
            }

            if(LegacyRando.globalSettings.StagNestBell) {
                lmb.DoLogicEdit(new("Can_Stag", "ORIG | Cliffs_03[right1] + Can_Replenish_Geo"));
            }            
        }

        private static void ConnectionPatch(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!LegacyRando.globalSettings.Any)
                return;

            Assembly assembly = Assembly.GetExecutingAssembly();
            JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
            
            using Stream stream = assembly.GetManifestResourceStream("LegacyRando.Resources.connectionOverrides.json");
            StreamReader reader = new(stream);
            List<ConnectionLogicObject> objectList = jsonSerializer.Deserialize<List<ConnectionLogicObject>>(new JsonTextReader(reader));

            foreach (ConnectionLogicObject o in objectList)
            {
                foreach (var sub in o.logicSubstitutions)
                {
                    bool exists = lmb.LogicLookup.TryGetValue(o.name, out _);
                    if (exists)
                        lmb.DoSubst(new(o.name, sub.Key, sub.Value));  
                }

                if (o.logicOverride != "")
                {
                    bool exists = lmb.LogicLookup.TryGetValue(o.name, out _);
                    if (exists)
                        lmb.DoLogicEdit(new(o.name, o.logicOverride));
                }
            }
        }
    }
}
