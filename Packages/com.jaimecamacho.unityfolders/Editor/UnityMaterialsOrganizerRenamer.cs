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

        CreateMaterialOrganizeAndRenameWizard.Show(folderPath);
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

    private class CreateMaterialOrganizeAndRenameWizard : ScriptableWizard
    {
        public string newFolderName = "New Folder";
        private string folderPath;

        public static void Show(string folderPath)
        {
            var wizard = DisplayWizard<CreateMaterialOrganizeAndRenameWizard>(
                "Crear Material, Ordenar y Renombrar", "Aplicar", "Cancelar");
            wizard.folderPath = folderPath;
        }

        private void OnWizardCreate()
        {
            UnityMaterialsCreator.CreateMaterialsInFolder(folderPath);

            string newFolderPath = UnityFolderOrganizer.OrganizeInChildFolder(folderPath, newFolderName);
            if (string.IsNullOrEmpty(newFolderPath))
                return;

            UnityAssetsRenamer.RenameAssetsInFolder(newFolderPath);

            Debug.Log($"Materiales creados, carpeta organizada y assets renombrados en {newFolderPath}.");
        }
    }
}
