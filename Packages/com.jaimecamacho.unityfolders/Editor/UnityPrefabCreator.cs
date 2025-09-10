using UnityEditor;
using UnityEngine;
using System.IO;

public static class UnityPrefabCreator
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear Prefab", false, 25)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear Prefab", false, 25)]
    private static void CreatePrefabMenu()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para crear el prefab.");
            return;
        }

        CreatePrefabWizard.Show(folderPath);
    }

    internal static void CreatePrefabsInFolder(string folderPath)
    {
        string meshFolder = Path.Combine(folderPath, "Mesh");
        string prefabFolder = Path.Combine(folderPath, "Prefab");

        if (!AssetDatabase.IsValidFolder(prefabFolder))
            AssetDatabase.CreateFolder(folderPath, "Prefab");

        string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { meshFolder });
        foreach (string guid in modelGuids)
        {
            string modelPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (model == null)
                continue;

            string prefix = model.name;
            if (prefix.EndsWith("_Geo"))
                prefix = prefix.Substring(0, prefix.Length - 4);

            string prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(prefabFolder, prefix + ".prefab"));
            GameObject instance = PrefabUtility.InstantiatePrefab(model) as GameObject;
            instance.name = prefix;
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Object.DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
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

    private class CreatePrefabWizard : ScriptableWizard
    {
        public string newFolderName = "New Folder";
        private string folderPath;

        public static void Show(string folderPath)
        {
            var wizard = DisplayWizard<CreatePrefabWizard>(
                "Crear Prefab", "Aplicar", "Cancelar");
            wizard.folderPath = folderPath;
        }

        private void OnWizardCreate()
        {
            UnityMaterialsCreator.CreateMaterialsInFolder(folderPath);

            string newFolderPath = UnityFolderOrganizer.OrganizeInChildFolder(folderPath, newFolderName);
            if (string.IsNullOrEmpty(newFolderPath))
                return;

            UnityAssetsRenamer.RenameAssetsInFolder(newFolderPath);

            CreatePrefabsInFolder(newFolderPath);

            Debug.Log($"Materiales, prefab y assets listos en {newFolderPath}.");
        }
    }
}
