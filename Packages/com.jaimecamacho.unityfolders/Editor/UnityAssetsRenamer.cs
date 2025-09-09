using UnityEditor;
using UnityEngine;
using System.IO;

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

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(assetPath))
                continue;

            string directory = Path.GetDirectoryName(assetPath);
            string extension = Path.GetExtension(assetPath);
            string nameNoExt = Path.GetFileNameWithoutExtension(assetPath);

            if (nameNoExt.StartsWith(rootName + "_"))
                continue;

            int underscoreIndex = nameNoExt.IndexOf('_');
            string suffix = underscoreIndex >= 0 ? nameNoExt.Substring(underscoreIndex + 1) : nameNoExt;

            string newName = rootName + "_" + suffix;
            string newPath = Path.Combine(directory, newName + extension);
            newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
            AssetDatabase.MoveAsset(assetPath, newPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Assets en la carpeta {folderPath} han sido renombrados.");
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

