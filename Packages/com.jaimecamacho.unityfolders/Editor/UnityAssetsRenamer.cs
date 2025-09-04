using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class UnityAssetsRenamer
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Renombrar Assets", false, 21)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Renombrar Assets", false, 21)]
    private static void RenameAssetsMenu()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para renombrar sus assets.");
            return;
        }

        RenameAssetsInFolder(folderPath);
    }

    private static void RenameAssetsInFolder(string folderPath)
    {
        string rootName = Path.GetFileName(folderPath);
        string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });

        var assets = new List<(string path, string type)>();
        var typeCounts = new Dictionary<string, int>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(assetPath))
                continue;

            string type = GetAssetType(assetPath);
            if (type == null)
                continue;

            assets.Add((assetPath, type));
            if (!typeCounts.ContainsKey(type))
                typeCounts[type] = 0;
            typeCounts[type]++;
        }

        foreach (var asset in assets)
        {
            string directory = Path.GetDirectoryName(asset.path);
            string extension = Path.GetExtension(asset.path);
            string nameNoExt = Path.GetFileNameWithoutExtension(asset.path);

            string baseName = rootName + "_" + asset.type;
            string newName = baseName;

            if (typeCounts[asset.type] > 1)
            {
                if (nameNoExt.StartsWith(rootName + "_"))
                {
                    nameNoExt = nameNoExt.Substring(rootName.Length + 1);
                }
                newName = baseName + "_" + nameNoExt;
            }

            if (nameNoExt == baseName || Path.GetFileNameWithoutExtension(asset.path) == newName)
                continue;

            string newPath = Path.Combine(directory, newName + extension);
            newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
            AssetDatabase.MoveAsset(asset.path, newPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Assets en la carpeta {folderPath} han sido renombrados.");
    }

    private static string GetAssetType(string assetPath)
    {
        string extension = Path.GetExtension(assetPath).ToLowerInvariant();

        switch (extension)
        {
            case ".anim":
            case ".controller":
            case ".overridecontroller":
                return "Animation";
            case ".fbx":
                var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (importer != null && ((importer.clipAnimations != null && importer.clipAnimations.Length > 0) ||
                                         (importer.defaultClipAnimations != null && importer.defaultClipAnimations.Length > 0)))
                    return "Animation";
                return "Mesh";
            case ".wav":
            case ".mp3":
            case ".ogg":
            case ".aiff":
                return "Audio";
            case ".mat":
                return "Material";
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
                return "Texture";
            case ".obj":
            case ".blend":
            case ".mesh":
                return "Mesh";
            case ".prefab":
                return "Prefab";
            case ".cs":
            case ".js":
            case ".boo":
                return "Script";
            case ".shader":
            case ".cginc":
            case ".shadergraph":
            case ".shadersubgraph":
            case ".compute":
            case ".hlsl":
                return "Shader";
            case ".vfx":
            case ".vfxgraph":
                return "VFX";
            default:
                return null;
        }
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}

