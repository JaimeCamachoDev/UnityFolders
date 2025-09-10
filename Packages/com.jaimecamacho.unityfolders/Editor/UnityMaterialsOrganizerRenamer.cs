using UnityEditor;
using UnityEngine;
using System.IO;

public static class UnityMaterialsOrganizerRenamer
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear Material, Ordenar y Renombrar", false, 24)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear Material, Ordenar y Renombrar", false, 24)]
    private static void CreateMaterialOrganizeAndRenameMenu()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para crear materiales, organizar y renombrar.");
            return;
        }

        UnityMaterialsCreator.CreateMaterialsInFolder(folderPath);
        UnityFolderOrganizer.OrganizeFolder(folderPath);
        UnityAssetsRenamer.RenameAssetsInFolder(folderPath);

        Debug.Log($"Materiales creados, carpeta organizada y assets renombrados en {folderPath}.");
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
