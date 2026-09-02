using System;
using Newtonsoft.Json;
using UnityEngine;
using ItemChanger;
using ItemChanger.Internal;

namespace MiscRando {
    [Serializable]
    public class MiscSprite: ISprite {
        private static SpriteManager EmbeddedSpriteManager = new(typeof(MiscSprite).Assembly, "MiscRando.Resources.");

        public string key;
        public MiscSprite(string key) {
            this.key = key;
        }

        [JsonIgnore]
        public Sprite Value => EmbeddedSpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
