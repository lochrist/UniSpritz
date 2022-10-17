using UnityEngine;

namespace UniMini
{
    class SpriteSheet
    {
        public Texture2D texture;
        public SpriteDesc[] spriteDescriptors;
        public Vector4[] uvs;

        public SpriteSheet()
        {
            spriteDescriptors = new SpriteDesc[0];
            uvs = new Vector4[0];
        }

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

            for (var i = 0; i < sprites.Length; ++i)
            {
                var sprite = sprites[i];
                var uv = ComputeUV(spritesheet, sprite.rect);
                spritesheet.spriteDescriptors[i] = new SpriteDesc()
                {
                    id = new SpriteId(sprite.name),
                    uv = uv,
                    rect = sprite.rect
                };
                spritesheet.uvs[i] = uv;
            }

            return spritesheet;
        }

        public static Vector4 ComputeUV(SpriteSheet sheet, Rect spriteRect)
        {
            var w = sheet.texture.width;
            var h = sheet.texture.height;
            float tilingX = 1f / (w / spriteRect.width);
            float tilingY = 1f / (h / spriteRect.height);
            float offsetX = tilingX * (spriteRect.x / spriteRect.width);
            float offsetY = tilingY * (spriteRect.y / spriteRect.height);
            return new Vector4(tilingX, tilingY, offsetX, offsetY);
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