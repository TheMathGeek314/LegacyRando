using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Modules;

namespace LegacyRando {
    public class LegacyModule: Module {
        internal static GameObject totemPrefab;
        internal static GameObject totemBasePrefab;
        internal static GameObject tollGatePrefab;
        internal static GameObject tollMachinePrefab;
        internal static GameObject bellPrefab;

        private static UnityAction<Scene, Scene> OnSceneLoad = null;

        private bool stagnested = false;

        public static void EarlyHook(Scene arg0, Scene arg1) {
            OnSceneLoad?.Invoke(arg0, arg1);
        }

        public override void Initialize() {
            OnSceneLoad += SceneChange;
            if(LegacyRando.localSettings.StagNestBell && RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.Stags) {
                stagnested = true;
                Events.AddFsmEdit("Cliffs_03", new FsmID("Stag", "Stag Control"), EditStagControl);
                Events.AddFsmEdit("Cliffs_03", new FsmID("UI List Stag", "ui_list"), EditUiList);
                Events.AddFsmEdit("Cliffs_03", new FsmID("Station Bell", "Stag Bell"), EditStagBell);
            }
        }

        public override void Unload() {
            OnSceneLoad -= SceneChange;
            if(stagnested) {
                Events.RemoveFsmEdit("Cliffs_03", new FsmID("Stag", "Stag Control"), EditStagControl);
                Events.RemoveFsmEdit("Cliffs_03", new FsmID("UI List Stag", "ui_list"), EditUiList);
                Events.RemoveFsmEdit("Cliffs_03", new FsmID("Station Bell", "Stag Bell"), EditStagBell);
                stagnested = false;
            }
        }

        private void SceneChange(Scene arg0, Scene arg1) {
            if(arg1.name == "Deepnest_41" && LegacyRando.localSettings.DeepnestToll) {
                GameObject machine = GameObject.Instantiate(tollMachinePrefab, new Vector3(99.8f, 88.1172f, 0.009f), Quaternion.identity);
                machine.name = "Toll Machine";
                machine.SetActive(true);
                GameObject gate = GameObject.Instantiate(tollGatePrefab, new Vector3(95.6827f, 87.9758f, -0.01f), Quaternion.identity);
                gate.name = "Toll Gate";
                gate.SetActive(true);
            }
            if(arg1.name == "Fungus1_03" && LegacyRando.localSettings.GreenpathTotem) {
                GameObject totem = GameObject.Instantiate(totemPrefab, new Vector3(54.04f, 12.627f, 0.01f), Quaternion.identity);
                totem.name = "LegacyRando Totem";
                totem.SetActive(true);
                GameObject.Instantiate(totemBasePrefab, new Vector3(54.07f, 11.1036f, 0.009f), Quaternion.identity).SetActive(true);
            }
            if(arg1.name == "Cliffs_03" && LegacyRando.localSettings.StagNestBell) {
                foreach(string name in new string[] { "bell_appear_broken", "bell0000" }) {
                    GameObject.Find(name).SetActive(false);
                }
                GameObject bell = GameObject.Instantiate(bellPrefab, new Vector3(19.8f, 7.1172f, 0.009f), Quaternion.identity);
                bell.LocateMyFSM("Stag Bell").FsmVariables.GetFsmString("PlayerData Bool").Value = nameof(PlayerData.openedStagNest);
                bell.name = "Station Bell";
                bell.SetActive(true);
            }
        }

        private void EditStagControl(PlayMakerFSM self) {
            FsmState openGrate = self.GetState("Open Grate");
            openGrate.RemoveActionsOfType<SetPlayerDataBool>();
            openGrate.RemoveActionsOfType<SetBoolValue>();
            FsmBool cancelTravel = self.AddFsmBool("Cancel Travel", false);
            if(!PlayerData.instance.GetBool("openedStagNest")) {
                self.FsmVariables.GetFsmInt("Station Position Number").Value = 0;
                self.GetState("Current Location Check").RemoveActionsOfType<IntCompare>();
                FsmState checkResult = self.GetState("Check Result");
                checkResult.AddFirstAction(new Lambda(() => {
                    if(cancelTravel.Value)
                        self.SendEvent("CANCEL");
                }));
                checkResult.AddTransition("CANCEL", "HUD Return");
            }
            self.GetState("HUD Return").AddFirstAction(new SetBoolValue {
                boolVariable = cancelTravel,
                boolValue = false
            });
        }

        private void EditUiList(PlayMakerFSM self) {
            self.GetState("Selection Made Cancel").AddFirstAction(new Lambda(() => {
                GameObject.Find("Stag").LocateMyFSM("Stag Control").FsmVariables.GetFsmBool("Cancel Travel").Value = true;
            }));
        }

        private void EditStagBell(PlayMakerFSM self) {
            FsmState init = self.GetState("Init");
            init.RemoveActionsOfType<PlayerDataBoolTest>();
            init.AddTransition("FINISHED", "Opened");
        }
    }
}
