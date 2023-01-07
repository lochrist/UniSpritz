using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using static UniMini.SimpleTexturePacker;

namespace UniMini
{
    public struct LoadTextureOptions
    {
        public string srcFolder;
        public string outputTexturePath;
        public bool recursive;
        public string textureMatchPattern;

        public LoadTextureOptions(string srcFolder, string outputTexturePath)
        {
            this.srcFolder = srcFolder;
            this.outputTexturePath = outputTexturePath;
            recursive = true;
            textureMatchPattern = "*.png";
        }
    }

    public struct PackTextureOptions
    {
        public string texturePathSuffix;
        public int outputWidth;
        public int outputHeight;
        public bool doForceSize;
        public int forceWidth;
        public int forceHeight;
        public int pixelPerUnit;

        public bool hasOutputSize => outputWidth > 0 && outputHeight > 0;

        public PackTextureOptions(int outputSize)
            : this(outputSize, outputSize)
        {
        }

        public PackTextureOptions(int w, int h)
        {
            outputWidth = w;
            outputHeight = h;
            doForceSize = false;
            forceWidth = -1;
            forceHeight = -1;
            pixelPerUnit = 32;
            texturePathSuffix = null;
        }

        public void SetOutputSize(int size)
        {
            outputWidth = size;
            outputHeight = size;
        }

        public void SetForceInputSize(int w, int h)
        {
            doForceSize = true;
            forceWidth = w;
            forceHeight = h;
        }
    }

    #region Packer
    // TODO: other packer ideas:
    // Efficient Packer with space list: https://github.com/aslushnikov/spritesheet-assembler/blob/master/lib/Packer.js
    // GrowingPacker using tree: https://github.com/jakesgordon/bin-packing/blob/master/js/packer.growing.js
    // Packer using tree: https://github.com/jakesgordon/bin-packing/blob/master/js/packer.js
    public class SimpleTexturePacker
    {
        public class PackNode
        {
            public Texture2D tex;
            public RectInt texRect;

            public RectInt rect;
            public PackNode right;
            public PackNode down;
            public bool used;

            public override string ToString()
            {
                var name = tex != null ? tex.name : "null";
                return $"{name} {rect}";
            }
        }

        public PackNode root;
        public PackTextureOptions opts;

        public SimpleTexturePacker(PackTextureOptions opts)
        {
            this.opts = opts;
            root = new PackNode()
            { 
                rect = new RectInt(0, 0, opts.outputWidth, opts.outputHeight)
            };
        }

        public IEnumerable<PackNode> Fit(Texture2D[] textures)
        {
            var output = new List<PackNode>(textures.Length);
            int count = 0;
            foreach(var t in textures)
            {
                var w = opts.doForceSize ? opts.forceWidth : t.width;
                var h = opts.doForceSize ? opts.forceHeight : t.height;
                var n = FindNode(root, w, h);
                if (n != null)
                {
                    UseNode(n, w, h);
                    n.tex = t;
                    output.Add(n);
                }
                count++;
            }
            return output;
        }

        private PackNode FindNode(PackNode parent, int w, int h)
        {
            if (parent == null)
                return null;
            if (parent.used)
            {
                var n = FindNode(parent.right, w, h);
                if (n != null)
                    return n;
                return FindNode(parent.down, w, h);
            }

            if (w <= parent.rect.width && h <= parent.rect.height)
                return parent;
            return null;
        }

        private void LogNodes(PackNode n, StringBuilder b = null)
        {
            b = b ?? new StringBuilder();

            if (n == null)
            {
                b.AppendLine("null");
            }
            else
                b.AppendLine(n.ToString());
            LogNodes(n.right, b);
            LogNodes(n.down, b);
        }

        private void UseNode(PackNode n, int w, int h)
        {
            n.texRect = new RectInt(n.rect.x, n.rect.y, w, h);
            if (n.tex != null)
            {
                UnityEngine.Debug.Log("already assigned");
            }
            n.down = new PackNode()
            {
                rect = new RectInt(n.rect.x, n.rect.y + h, n.rect.width, n.rect.height - h)
            };
            n.right = new PackNode()
            {
                rect = new RectInt(n.rect.x + w, n.rect.y, n.rect.width - w, h)
            };
            n.used = true;
        }
    }
    #endregion

    public static class SpritzTextureUtil
    {
        public static void AddTransparency(Texture2D src)
        {
            var buffer = src.GetPixels32();
            for (var i = 0; i < buffer.Length; ++i)
            {
                Color32 c = buffer[i];
                if (c.r == 0 && c.g == 0 && c.b == 0 && c.a == 255)
                {
                    buffer[i] = new Color32(0, 0, 0, 0);
                }
            }
            src.SetPixels32(buffer);
            src.Apply();
        }

        public static void AddTransparency(string src, string output)
        {
            var srcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(src);
            if (!srcTex)
                throw new System.Exception($"Invalid texture path: {src}");

            AddTransparency(srcTex);

            var bytes = srcTex.EncodeToPNG();
            File.WriteAllBytes(output, bytes);
        }

        public static void ProcessSpritzTextures(Texture2D[] textures)
        {
            foreach(var t in textures)
            {
                var path = AssetDatabase.GetAssetPath(t);
                if (string.IsNullOrEmpty(path))
                    continue;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                    continue;
                importer.textureType = TextureImporterType.Sprite;
                importer.isReadable = true;
                if (importer.spriteImportMode != SpriteImportMode.Multiple)
                    importer.spriteImportMode = SpriteImportMode.Single;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.spritePixelsPerUnit = 32;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            AssetDatabase.Refresh();
        }

        public static void CreateSpriteSheet(Texture2D[] textures, string spriteSheetPath, PackTextureOptions opts)
        {
            var alreadyExisting = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(spriteSheetPath);
            if (alreadyExisting)
            {
                AssetDatabase.DeleteAsset(spriteSheetPath);
            }

            AssetDatabase.Refresh();
            var outputTexture = PackSpritesInTexture(textures, out var spriteDatas, opts);
            if (spriteDatas == null)
            {
                throw new System.Exception($"Cannot pack textures in {spriteSheetPath}");
            }

            var bytes = outputTexture.EncodeToPNG();
            File.WriteAllBytes(spriteSheetPath, bytes);

            AssetDatabase.Refresh();

            var importer = AssetImporter.GetAtPath(spriteSheetPath) as TextureImporter;
            if (importer == null)
                throw new System.Exception($"Cannot get importer for {spriteSheetPath}");
            importer.textureType = TextureImporterType.Sprite;
            importer.isReadable = true;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = opts.pixelPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.spritesheet = spriteDatas;
            AssetDatabase.Refresh();
            importer.SaveAndReimport();

            AssetDatabase.Refresh();
        }

        public static void CreateSpriteSheet(LoadTextureOptions loadTextureOpts, PackTextureOptions packOpts)
        {
            if (!Directory.Exists(loadTextureOpts.srcFolder))
                throw new System.Exception($"Cannot find folder {loadTextureOpts.srcFolder}");

            var textures = LoadSingleTexturesInFolder(loadTextureOpts);
            CreateSpriteSheet(textures, loadTextureOpts.outputTexturePath, packOpts);
        }

        public static Texture2D PackSpritesInTexture(Texture2D[] inputTextures, out SpriteMetaData[] metaData, PackTextureOptions opts)
        {
            ProcessSpritzTextures(inputTextures);
            var firstTex = inputTextures[0];
            if (!opts.doForceSize && !inputTextures.All(t => t.width == firstTex.width && t.height == firstTex.height))
            {
                System.Array.Sort(inputTextures, (a, b) => (a.width * a.height) - (b.width * b.height));
            }

            IEnumerable<PackNode> packData;
            if (!opts.hasOutputSize)
            {
                // Try to establish a good size:
                var totalSpritesArea = inputTextures.Sum(t => t.width * t.height);
                var outputSize = (int)(Mathf.Sqrt(totalSpritesArea) + 1);
                do
                {
                    outputSize = FindNextPowerOf2(outputSize);
                    opts.SetOutputSize(outputSize);
                    var packer = new SimpleTexturePacker(opts);
                    packData = packer.Fit(inputTextures);
                } while (packData.Count() < inputTextures.Length);
            }
            else
            {
                var packer = new SimpleTexturePacker(opts);
                packData = packer.Fit(inputTextures);
            }

            var outTex = new Texture2D(opts.outputWidth, opts.outputHeight);
            metaData = packData.Select(pd =>
            {
                SpriteMetaData data = new SpriteMetaData();
                data.pivot = new Vector2(0.5f, 0.5f);
                data.alignment = 9;
                var textPath = AssetDatabase.GetAssetPath(pd.tex);
                if (!string.IsNullOrEmpty(textPath) && !string.IsNullOrEmpty(opts.texturePathSuffix) && textPath.StartsWith(opts.texturePathSuffix))
                {
                    var textPartialPath = textPath.Substring(opts.texturePathSuffix.Length + (opts.texturePathSuffix.EndsWith("/") ? 0 : 1));
                    textPartialPath = textPartialPath.Replace("/", "_");
                    textPartialPath = Path.GetFileNameWithoutExtension(textPartialPath);
                    data.name = textPartialPath;
                }
                else
                {
                    data.name = pd.tex.name;
                }
                
                var dstY = outTex.height - pd.texRect.y - pd.texRect.height;
                data.rect = new Rect(pd.texRect.x, dstY, pd.texRect.width, pd.texRect.height);

                if (opts.doForceSize && 
                    (opts.forceWidth != pd.tex.width || opts.forceHeight != pd.tex.height))
                {
                    var inputW = Mathf.Min(opts.forceWidth, pd.tex.width);
                    var inputH = Mathf.Min(opts.forceHeight, pd.tex.height);
                    var dstX = pd.texRect.x;
                    if (inputW < opts.forceWidth)
                    {
                        var offset = (opts.forceWidth - inputW) / 2;
                        dstX += offset;
                    }
                    if (inputH < opts.forceHeight)
                    {
                        var offset = (opts.forceHeight - inputH) / 2;
                        dstY += offset;
                    }
                    Graphics.CopyTexture(pd.tex, 0, 0, 0, 0, inputW, inputH, // Src
                        outTex, 0, 0, dstX, dstY); // Dst
                }
                else
                {
                    Graphics.CopyTexture(pd.tex, 0, 0, 0, 0, pd.tex.width, pd.tex.height, // Src
                        outTex, 0, 0, pd.texRect.x, dstY); // Dst
                }
                

                return data;
            }).ToArray();

            if (packData.Count() < inputTextures.Length)
            {
                Debug.LogWarning($"{inputTextures.Length - packData.Count()} sprites haven't been packed.");
            }

            return outTex;
        }

        #region MenuItems
        [MenuItem("Assets/Spritz/Add Transparency to", true)]
        static bool AddTransparencyToValidate()
        {
            var textures = GetTextureInSelection();
            return textures != null && textures.Length > 0;
        }

        [MenuItem("Assets/Spritz/Add Transparency to")]
        static void AddTransparencyTo()
        {
            var texturePaths = GetTextureInSelection().Select(t => AssetDatabase.GetAssetPath(t)).Where(p => !string.IsNullOrEmpty(p));
            foreach(var p in texturePaths)
                AddTransparency(p, p);
        }

        [MenuItem("Assets/Spritz/Create SpriteSheet From Folder", true)]
        static bool CreateSpriteSheetFromFolderValidate()
        {
            return IsSelectedObjectAValidDirectory(out var path);
        }

        [MenuItem("Assets/Spritz/Create SpriteSheet From Folder")]
        static void CreateSpriteSheetFromFolder()
        {
            if (!IsSelectedObjectAValidDirectory(out var path))
                throw new System.Exception("Selection doesn't contain a folder");

            var folderName = Path.GetFileName(path);
            var spriteSheetPath = Path.Join(path, folderName + ".png");
            var loadTextureOpts = new LoadTextureOptions(path, spriteSheetPath);
            var packOpts = new PackTextureOptions(-1)
            {
                texturePathSuffix = path,
                pixelPerUnit = 48
            };
            packOpts.SetForceInputSize(48, 48);
            CreateSpriteSheet(loadTextureOpts, packOpts);
        }

        [MenuItem("Assets/Spritz/Process Textures in Folder", true)]
        static bool ProcessTexturesInFolderValidate()
        {
            return IsSelectedObjectAValidDirectory(out var path);
        }

        [MenuItem("Assets/Spritz/Process Textures in Folder")]
        static void ProcessTexturesInFolder()
        {
            if (!IsSelectedObjectAValidDirectory(out var path))
                throw new System.Exception("Selection doesn't contain a folder");

            var loadTextureOpts = new LoadTextureOptions(path, null);
            var textures = LoadSingleTexturesInFolder(loadTextureOpts);
            ProcessSpritzTextures(textures);
        }
        #endregion

        #region Impl
        private static bool IsSelectedObjectAValidDirectory(out string dirPath)
        {
            dirPath = "";
            if (!(Selection.activeObject is DefaultAsset))
            {
                return false;
            }

            dirPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))
            {
                return false;
            }

            return true;
        }

        private static Texture2D[] LoadSingleTexturesInFolder(LoadTextureOptions loadTextureOpts)
        {
            var potentialImgPaths = Directory.GetFiles(loadTextureOpts.srcFolder, loadTextureOpts.textureMatchPattern, loadTextureOpts.recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToArray();
            var textures = potentialImgPaths.Select(p =>
            {
                var t = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
                if (t)
                {
                    var importer = AssetImporter.GetAtPath(p) as TextureImporter;
                    // Filter out multi sprite textures:
                    if (importer == null || importer.spriteImportMode != SpriteImportMode.Multiple)
                    {
                        return t;
                    }
                }
                return null;
            }).Where(a => a != null).ToArray();
            return textures;
        }

        private static Texture2D[] GetTextureInSelection()
        {
            return Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        }

        internal static int FindNextPowerOf2(float f)
        {
            var powerOf2 = 1;
            while (powerOf2 <= f)
            {
                powerOf2 = powerOf2 * 2;
            }
            return powerOf2;
        }
        #endregion
    }
}
