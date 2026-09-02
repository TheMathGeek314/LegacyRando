using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using ItemChanger.Modules;

namespace MiscRando {
    public class MiscModule: Module {
        internal static GameObject totemPrefab;
        internal static GameObject totemBasePrefab;
        internal static GameObject tollGatePrefab;
        internal static GameObject tollMachinePrefab;
        internal static GameObject bellPrefab;

        private static UnityAction<Scene, Scene> OnSceneLoad = null;

        public static void EarlyHook(Scene arg0, Scene arg1) {
            OnSceneLoad?.Invoke(arg0, arg1);
        }

        public override void Initialize() {
            OnSceneLoad += SceneChange;
        }

        public override void Unload() {
            OnSceneLoad -= SceneChange;
        }

        private void SceneChange(Scene arg0, Scene arg1) {
            if(arg1.name == "Deepnest_41" && MiscRando.localSettings.DeepnestToll) {
                GameObject machine = GameObject.Instantiate(tollMachinePrefab, new Vector3(99.8f, 88.1172f, 0.009f), Quaternion.identity);
                machine.name = "Toll Machine";
                machine.SetActive(true);
                GameObject gate = GameObject.Instantiate(tollGatePrefab, new Vector3(95.6827f, 87.9758f, -0.01f), Quaternion.identity);
                gate.name = "Toll Gate";
                gate.SetActive(true);
            }
            if(arg1.name == "Fungus1_03" && MiscRando.localSettings.GreenpathTotem) {
                GameObject totem = GameObject.Instantiate(totemPrefab, new Vector3(54.04f, 12.627f, 0.01f), Quaternion.identity);
                totem.name = "MiscRando Totem";
                totem.SetActive(true);
                GameObject.Instantiate(totemBasePrefab, new Vector3(54.07f, 11.1036f, 0.009f), Quaternion.identity).SetActive(true);
            }
            if(arg1.name == "Cliffs_03" && MiscRando.localSettings.StagNestBell) {
                foreach(string name in new string[] { "bell_appear_broken", "bell0000" }) {
                    GameObject.Find(name).SetActive(false);
                }
                GameObject bell = GameObject.Instantiate(bellPrefab, new Vector3(19.8f, 7.1172f, 0.009f), Quaternion.identity);
                bell.LocateMyFSM("Stag Bell").FsmVariables.GetFsmString("PlayerData Bool").Value = nameof(PlayerData.openedStagNest);
                bell.SetActive(true);
            }
        }
    }
}
