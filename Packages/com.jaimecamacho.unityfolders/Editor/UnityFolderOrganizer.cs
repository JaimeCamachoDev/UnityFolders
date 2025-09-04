using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public static class UnityFolderOrganizer
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Ordenar carpeta", false, 20)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Ordenar carpeta", false, 20)]
    private static void OrganizeFolder()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para ordenar.");
            return;
        }

        string[] subfolders = new string[]
        {
            "Animation",
            "Audio",
            "Material",
            "Mesh",
            "Prefab",
            "Script",
            "Shader",
            "VFX"
        };

        // Crear subcarpetas si no existen
        foreach (string subfolder in subfolders)
        {
            string subfolderPath = Path.Combine(folderPath, subfolder);
            if (!AssetDatabase.IsValidFolder(subfolderPath))
            {
                AssetDatabase.CreateFolder(folderPath, subfolder);
            }
        }

        // Obtener todos los assets dentro de la carpeta seleccionada
        string[] assetGuids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Saltar carpetas
            if (AssetDatabase.IsValidFolder(assetPath))
                continue;

            string extension = Path.GetExtension(assetPath).ToLowerInvariant();
            string destinationFolderName = GetDestinationFolderName(extension);

            if (destinationFolderName == null)
                continue;

            string destinationPath = Path.Combine(folderPath, destinationFolderName, Path.GetFileName(assetPath));
            destinationPath = AssetDatabase.GenerateUniqueAssetPath(destinationPath);
            AssetDatabase.MoveAsset(assetPath, destinationPath);
        }

        RemoveEmptySubdirectories(folderPath);
        AssetDatabase.Refresh();
        Debug.Log($"Carpeta {folderPath} organizada.");
    }

    private static string GetDestinationFolderName(string extension)
    {
        switch (extension)
        {
            case ".anim":
                return "Animation";
            case ".wav":
            case ".mp3":
            case ".ogg":
            case ".aiff":
                return "Audio";
            case ".mat":
                return "Material";
            case ".fbx":
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
                return "Shader";
            case ".vfx":
            case ".vfxgraph":
                return "VFX";
            default:
                return null;
        }
    }

    private static void RemoveEmptySubdirectories(string rootPath)
    {
        var directories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
                                   .OrderByDescending(d => d.Length);
        foreach (string directory in directories)
        {
            if (Directory.GetFileSystemEntries(directory).Length == 0)
            {
                AssetDatabase.DeleteAsset(directory);
            }
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

