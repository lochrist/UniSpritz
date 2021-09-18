using UnityEngine;

namespace UniMini
{
    struct SpriteDesc
    {
        public SpriteId id;
        public Vector4 uv;
        public bool isValid => id.isValid;
    }

    class SpriteSheet
    {
        public Texture2D texture;
        public SpriteDesc[] spriteDescriptors;
        public Vector4[] uvs;

        public static SpriteSheet CreateFromResource(string resourcePath)
        {
            var sprites = Resources.LoadAll<Sprite>(resourcePath);
            if (sprites == null || sprites.Length == 0)
                throw new System.Exception($"Cannot find sprites in resource {resourcePath}");
            return CreateFromSprites(sprites);
        }

        public static SpriteSheet CreateFromSprites(Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0)
                throw new System.Exception("Cannot create sprite sheet from empty list of sprites");

            var spritesheet = new SpriteSheet();
            spritesheet.texture = sprites[0].texture;
            if (spritesheet.texture == null)
                throw new System.Exception("Cannot create sprite sheet from null texture");

            spritesheet.spriteDescriptors = new SpriteDesc[sprites.Length];
            spritesheet.uvs = new Vector4[sprites.Length];

            var w = spritesheet.texture.width;
            var h = spritesheet.texture.height;
            for (var i = 0; i < sprites.Length; ++i)
            {
                var sprite = sprites[i];
                float tilingX = 1f / (w / sprite.rect.width);
                float tilingY = 1f / (h / sprite.rect.height);
                float offsetX = tilingX * (sprite.rect.x / sprite.rect.width);
                float offsetY = tilingY * (sprite.rect.y / sprite.rect.height);
                var uv = new Vector4(tilingX, tilingY, offsetX, offsetY);
                spritesheet.spriteDescriptors[i] = new SpriteDesc()
                {
                    id = new SpriteId(sprite.name),
                    uv = uv
                };
                spritesheet.uvs[i] = uv;
            }

            return spritesheet;
        }

        public SpriteDesc GetSpriteAt(int index)
        {
            return spriteDescriptors[index];
        }

        public SpriteDesc GetSpriteById(SpriteId id)
        {
            for (var i = 0; i < spriteDescriptors.Length; ++i)
            {
                if (spriteDescriptors[i].id == id)
                {
                    return spriteDescriptors[i];
                }
            }

            return new SpriteDesc();
        }

        public SpriteDesc GetSpriteByName(string name)
        {
            return GetSpriteById(new SpriteId(name));
        }
    }
}