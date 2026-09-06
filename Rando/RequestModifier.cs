using ItemChanger;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace LegacyRando {
    internal class RequestModifier {
        public static void Hook() {
            RequestBuilder.OnUpdate.Subscribe(-100, ApplyTotemDef);
            RequestBuilder.OnUpdate.Subscribe(-100, ApplyTollDefs);
            RequestBuilder.OnUpdate.Subscribe(-499, SetupItems);
            RequestBuilder.OnUpdate.Subscribe(-499.5f, DefinePools);
            RequestBuilder.OnUpdate.Subscribe(0, CloneToLocal);
        }

        private static void ApplyTotemDef(RequestBuilder rb) {
            if(LegacyRando.globalSettings.GreenpathTotem && rb.gs.PoolSettings.SoulTotems) {
                rb.AddLocationByName(Consts.GreenpathTotem);
                rb.EditLocationRequest(Consts.GreenpathTotem, info => {
                    info.customPlacementFetch = (factory, placement) => {
                        if(factory.TryFetchPlacement(Consts.GreenpathTotem, out AbstractPlacement existingPlacement))
                            return existingPlacement;
                        AbstractLocation absLoc = Finder.GetLocation(Consts.GreenpathTotem);
                        absLoc.flingType = FlingType.Everywhere;
                        AbstractPlacement ap = absLoc.Wrap();
                        factory.AddPlacement(ap);
                        return ap;
                    };
                    info.getLocationDef = () => new() {
                        Name = Consts.GreenpathTotem,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false,
                        SceneName = "Fungus1_03"
                    };
                });
            }
        }

        private static void ApplyTollDefs(RequestBuilder rb) {
            foreach((string toll, bool setting) in new (string, bool)[] { (Consts.PeakToll, LegacyRando.globalSettings.PeakToll), (Consts.DeepnestToll, LegacyRando.globalSettings.DeepnestToll) }) {
                if(setting) {
                    rb.AddLocationByName(toll);
                    rb.EditLocationRequest(toll, info => {
                        info.customPlacementFetch = (factory, placement) => {
                            if(factory.TryFetchPlacement(toll, out AbstractPlacement existingPlacement))
                                return existingPlacement;
                            AbstractLocation absLoc = Finder.GetLocation(toll);
                            absLoc.flingType = FlingType.StraightUp;
                            AbstractPlacement ap = absLoc.Wrap();
                            factory.AddPlacement(ap);
                            return ap;
                        };
                        info.getLocationDef = () => new() {
                            Name = toll,
                            FlexibleCount = false,
                            AdditionalProgressionPenalty = false,
                            SceneName = toll == Consts.DeepnestToll ? "Deepnest_41" : "Mines_33"
                        };
                    });
                }
            }
        }

        private static void SetupItems(RequestBuilder rb) {
            GlobalSettings gs = LegacyRando.globalSettings;
            if(!gs.Any)
                return;
            if(gs.GreenpathTotem && rb.gs.PoolSettings.SoulTotems)
                rb.AddItemByName(ItemNames.Soul_Totem_C);
            foreach((string toll, bool enabled) in new (string, bool)[] { (Consts.PeakToll, gs.PeakToll), (Consts.DeepnestToll, gs.DeepnestToll) }) {
                rb.EditItemRequest(toll, info => {
                    info.getItemDef = () => new ItemDef {
                        Name = toll,
                        Pool = "Toll",
                        MajorItem = false,
                        PriceCap = 50
                    };
                });
                if(enabled)
                    rb.AddItemByName(toll);
                else
                    rb.AddToVanilla(toll, toll);
            }
        }

        private static void DefinePools(RequestBuilder rb) {
            GlobalSettings gs = LegacyRando.globalSettings;
            if(!gs.Any)
                return;
            if(rb.gs.SplitGroupSettings.RandomizeOnStart) {
                if(gs.TollGroup >= 0 && gs.TollGroup <= 2)
                    gs.TollGroup = rb.rng.Next(3);
            }
            ItemGroupBuilder myGroup = null;
            if(gs.TollGroup > 0) {
                string label = RBConsts.SplitGroupPrefix + gs.TollGroup;
                foreach(ItemGroupBuilder igb in rb.EnumerateItemGroups()) {
                    if(igb.label == label) {
                        myGroup = igb;
                        break;
                    }
                }
                myGroup ??= rb.MainItemStage.AddItemGroup(label);
            }

            rb.OnGetGroupFor.Subscribe(0.01f, ResolveLegacyGroup);
            bool ResolveLegacyGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb) {
                if(type is RequestBuilder.ElementType.Item or RequestBuilder.ElementType.Location) {
                    if(item.StartsWith("Toll-")) {
                        gb = myGroup;
                        return true;
                    }
                }
                gb = default;
                return false;
            }
        }

        private static void CloneToLocal(RequestBuilder rb) {
            LocalSettings ls = LegacyRando.localSettings;
            GlobalSettings gs = LegacyRando.globalSettings;
            ls.GreenpathTotem = gs.GreenpathTotem;
            ls.PeakToll = gs.PeakToll;
            ls.DeepnestToll = gs.DeepnestToll;
            ls.StagNestBell = gs.StagNestBell;
        }
    }
}
