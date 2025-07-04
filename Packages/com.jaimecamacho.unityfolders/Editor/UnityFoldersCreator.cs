using UnityEngine;
using UnityEditor;
using System.IO;

public static class UnityFoldersCreator
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear carpetas", false, 10)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear carpetas", false, 10)]
    private static void CreateUnityFolders()
    {
        string folderPath = GetSelectedPathOrFallback();

        string mainFolderName = "New Folder";
        string uniqueFolderPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, mainFolderName));
        AssetDatabase.CreateFolder(folderPath, Path.GetFileName(uniqueFolderPath));
        AssetDatabase.Refresh();

        CreateSubfolders(uniqueFolderPath);
        Debug.Log($"Carpeta creada en: {uniqueFolderPath}");
    }

    private static void CreateSubfolders(string parentFolderPath)
    {
        string[] subfolders = new string[]
        {
            "Animations",
            "Audio",
            "Materials",
            "Meshes",
            "Prefabs",
            "Scripts",
            "Shaders",
            "VFX"
        };

        foreach (string subfolder in subfolders)
        {
            AssetDatabase.CreateFolder(parentFolderPath, subfolder);
        }

        AssetDatabase.Refresh();
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
