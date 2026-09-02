using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;

namespace MiscRando {
    public class TollItem: AbstractItem {
        public TollItem(string name) {
            this.name = name;
            InteropTag tag = RandoInterop.AddTag(this);
            tag.Properties["PinSprite"] = new MiscSprite("TollPin");
            UIDef = new MsgUIDef {
                name = new BoxedString(name.Replace("-", " - ")),
                shopDesc = new BoxedString("To whom do I sell tolls? Time marches on."),
                sprite = new MiscSprite("TollPin")
            };
        }

        public override void GiveImmediate(GiveInfo info) {
            if(GameManager.instance.sceneName == (name == Consts.DeepnestToll ? "Deepnest_41" : "Mines_33")) {
                PlayMakerFSM.BroadcastEvent("MISCRANDO GATE OPEN");
            }
        }
    }
}
