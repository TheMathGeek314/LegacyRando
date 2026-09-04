using System;
using Newtonsoft.Json;
using UnityEngine;
using ItemChanger;
using ItemChanger.Internal;

namespace LegacyRando {
    [Serializable]
    public class LegacySprite: ISprite {
        private static SpriteManager EmbeddedSpriteManager = new(typeof(LegacySprite).Assembly, "LegacyRando.Resources.");

        public string key;
        public LegacySprite(string key) {
            this.key = key;
        }

        [JsonIgnore]
        public Sprite Value => EmbeddedSpriteManager.GetSprite(key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
