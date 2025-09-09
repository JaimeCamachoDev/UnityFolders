using UnityEditor;
using UnityEngine;
using System.IO;

public static class UnityOrganizeAndRenamer
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Ordenar y Renombrar", false, 22)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Ordenar y Renombrar", false, 22)]
    private static void OrganizeAndRenameMenu()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para organizar y renombrar.");
            return;
        }

        OrganizeAndRenameWizard.Show(folderPath);
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

    private class OrganizeAndRenameWizard : ScriptableWizard
    {
        public string newFolderName = "New Folder";
        private string folderPath;

        public static void Show(string folderPath)
        {
            var wizard = DisplayWizard<OrganizeAndRenameWizard>("Ordenar y Renombrar", "Aplicar", "Cancelar");
            wizard.folderPath = folderPath;
        }

        private void OnWizardCreate()
        {
            string newFolderPath = UnityFolderOrganizer.OrganizeInChildFolder(folderPath, newFolderName);
            if (!string.IsNullOrEmpty(newFolderPath))
            {
                UnityAssetsRenamer.RenameAssetsInFolder(newFolderPath);
            }
        }
    }
}
