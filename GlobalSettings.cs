using Newtonsoft.Json;

namespace MiscRando {
    public class GlobalSettings {
        public bool GreenpathTotem = false;
        public bool PeakToll = false;
        public bool DeepnestToll = false;

        [MenuChanger.Attributes.MenuRange(-1, 99)]
        public int TollGroup = -1;

        public bool StagNestBell = false;

        [JsonIgnore]
        public bool Any => GreenpathTotem || PeakToll || DeepnestToll || StagNestBell;
    }

    public class LocalSettings {
        public bool GreenpathTotem = false;
        public bool PeakToll = false;
        public bool DeepnestToll = false;
        public bool StagNestBell = false;
    }
}
