#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace VzFolders.Pipeline
{
    // Shared taxonomy used both when creating per-type asset folders and when
    // auto-organizing a chaotic folder, so the two features never drift apart.
    static class VzFoldersAssetTypeFolders
    {
        public const string Animation = "Animation";
        public const string Audio = "Audio";
        public const string Material = "Material";
        public const string Mesh = "Mesh";
        public const string Prefab = "Prefab";
        public const string Script = "Script";
        public const string Shader = "Shader";
        public const string VFX = "VFX";

        public static readonly string[] All = { Animation, Audio, Material, Mesh, Prefab, Script, Shader, VFX };

        // Returns the type folder an asset belongs to, or null if it should be left where it is.
        public static string GetFolderForAsset(string assetPath)
        {
            switch (assetPath.GetExtensionLower())
            {
                case ".anim":
                case ".controller":
                case ".overridecontroller":
                    return Animation;

                case ".fbx":
                case ".dae":
                case ".obj":
                case ".blend":
                    return HasAnimationClips(assetPath) ? Animation : Mesh;

                case ".wav":
                case ".mp3":
                case ".ogg":
                case ".aiff":
                    return Audio;

                case ".mat":
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".tga":
                case ".tif":
                case ".tiff":
                case ".psd":
                case ".bmp":
                case ".gif":
                case ".exr":
                case ".hdr":
                    return Material;

                case ".mesh":
                    return Mesh;

                case ".prefab":
                    return Prefab;

                case ".cs":
                case ".js":
                case ".boo":
                    return Script;

                case ".shader":
                case ".cginc":
                case ".shadergraph":
                case ".shadersubgraph":
                case ".compute":
                case ".hlsl":
                    return Shader;

                case ".vfx":
                case ".vfxgraph":
                    return VFX;

                default:
                    return null;
            }
        }

        static bool HasAnimationClips(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer == null) return false;

            return (importer.clipAnimations != null && importer.clipAnimations.Length > 0)
                || (importer.defaultClipAnimations != null && importer.defaultClipAnimations.Length > 0);
        }

        static string GetExtensionLower(this string path) => Path.GetExtension(path).ToLowerInvariant();
    }
}
#endif
