using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Locations;
using ItemChanger.Util;

namespace MiscRando {
    internal class TollLocation: AutoLocation {
        private static readonly Dictionary<string, TollLocation> SubscribedLocations = new();
        private static FieldInfo _textMesh = typeof(DialogueBox).GetField("textMesh", BindingFlags.NonPublic | BindingFlags.Instance);

        protected override void OnLoad() {
            if(SubscribedLocations.Count == 0)
                HookTolls();
            SubscribedLocations[UnsafeSceneName] = this;
        }

        protected override void OnUnload() {
            SubscribedLocations.Remove(UnsafeSceneName);
            if(SubscribedLocations.Count == 0)
                UnhookTolls();
        }

        private void HookTolls() {
            On.PlayMakerFSM.OnEnable += EditTolls;
        }

        private void UnhookTolls() {
            On.PlayMakerFSM.OnEnable -= EditTolls;
        }

        private void EditTolls(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            bool validTollRoom = false;
            AbstractPlacement ap = null;
            if(self.gameObject.scene.name == "Mines_33" && MiscRando.localSettings.PeakToll) {
                validTollRoom = true;
                ap = Ref.Settings.Placements[Consts.PeakToll];
            }
            if(self.gameObject.scene.name == "Deepnest_41" && MiscRando.localSettings.DeepnestToll) {
                validTollRoom = true;
                ap = Ref.Settings.Placements[Consts.DeepnestToll];
            }
            if(!validTollRoom || ap == null) {
                orig(self);
                return;
            }

            if(self.FsmName == "Toll Machine") {
                self.gameObject.GetComponent<PersistentBoolItem>().enabled = false;
                FsmState GateCheckState = self.AddState("MiscRando Gate Check");
                FsmState ActivatedQState = self.GetState("Activated?");
                self.GetState("Pause").RemoveTransitionsTo("Activated?");
                self.GetState("Pause").AddTransition("FINISHED", GateCheckState);
                GateCheckState.AddTransition("FINISHED", ActivatedQState);
                GateCheckState.Actions = [
                    new Lambda(() => {
                        if(RandomizerMod.RandomizerMod.RS.TrackerData.pm.Get(ap.Name) == 0) {
                            self.Fsm.Event("FINISHED");
                        }
                    }),
                    new SendEventByName {
                        eventTarget = new FsmEventTarget { target = FsmEventTarget.EventTarget.BroadcastAll },
                        sendEvent = "MISCRANDO GATE OPENED",
                        delay = 0,
                        everyFrame = false
                    }
                ];
                ActivatedQState.RemoveTransitionsTo("Activated");
                ActivatedQState.AddTransition("ACTIVATED", "Toll Gate Opened");
                ActivatedQState.Actions = [
                    new Lambda(() => {
                        if(ap.AllObtained()) {
                            self.Fsm.Event("ACTIVATED");
                        }
                    })
                ];
                FsmState SendTextState = self.GetState("Send Text");
                CallMethodProper cmp = SendTextState.GetFirstActionOfType<CallMethodProper>();
                cmp.Enabled = false;
                SendTextState.AddLastAction(new Lambda(() => {
                    DialogueBox db = cmp.gameObject.GameObject.Value.GetComponent<DialogueBox>();
                    db.currentPage = 1;
                    ((TextMeshPro)_textMesh.GetValue(db)).text = GetItemString(ap);
                    ((TextMeshPro)_textMesh.GetValue(db)).ForceMeshUpdate();
                    db.ShowPage(1);
                }));
                self.GetState("Open Gates").AddTransition("FINISHED", "Toll Gate Opened");
                self.GetState("Open Gates").Actions = [
                    new Lambda(() => {
                        GameObject shiny = ShinyUtility.MakeNewMultiItemShiny(ap, ap.Items, FlingType.Everywhere);
                        shiny.transform.position = self.gameObject.transform.position + Vector3.down;
                        shiny.SetActive(true);
                        ShinyUtility.FlingShinyRandomly(shiny.LocateMyFSM("Shiny Control"));
                    })
                ];
            }
            else if(self.FsmName == "Toll Gate") {
                self.GetState("Idle").AddTransition("MISCRANDO GATE OPEN", "Open");
                self.GetState("Idle").AddTransition("MISCRANDO GATE OPENED", "Destroy Self");
            }
            orig(self);
        }

        private string GetItemString(AbstractPlacement placement) {
            string output = placement.Items[0].UIDef.GetPostviewName();
            for(int i = 1; i < placement.Items.Count; i++)
                output += ", " + placement.Items[i].GetPreviewName();
            if(output.Length > 50)
                return output.Substring(0, 50) + "...";
            return output;
        }
    }
}
