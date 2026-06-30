using UnityEditor;
using UnityEngine;
using System.IO;

public static class UnityPrefabCreator
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear Prefab MAS", false, 25)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear Prefab MAS", false, 25)]
    private static void CreatePrefabMASMenu()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para crear el prefab.");
            return;
        }

        CreatePrefabWizard.Show(folderPath, PrefabCreationMode.MAS);
    }

    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear Prefab LIT", false, 26)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear Prefab LIT", false, 26)]
    private static void CreatePrefabLITMenu()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para crear el prefab.");
            return;
        }

        CreatePrefabWizard.Show(folderPath, PrefabCreationMode.LIT);
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

    private enum PrefabCreationMode
    {
        MAS,
        LIT
    }

    private class CreatePrefabWizard : ScriptableWizard
    {
        public string newFolderName = "New Folder";
        private string folderPath;
        private PrefabCreationMode mode;

        public static void Show(string folderPath, PrefabCreationMode mode)
        {
            string title = mode == PrefabCreationMode.MAS ? "Crear Prefab MAS" : "Crear Prefab LIT";
            var wizard = DisplayWizard<CreatePrefabWizard>(
                title, "Aplicar", "Cancelar");
            wizard.folderPath = folderPath;
            wizard.mode = mode;
        }

        private void OnWizardCreate()
        {
            switch (mode)
            {
                case PrefabCreationMode.MAS:
                    UnityMaterialsCreator.CreateMaterialsInFolder(folderPath);
                    break;
                case PrefabCreationMode.LIT:
                    UnityMaterialsCreator.CreateLitMaterialsInFolder(folderPath);
                    break;
            }

            string newFolderPath = UnityFolderOrganizer.OrganizeInChildFolder(folderPath, newFolderName);
            if (string.IsNullOrEmpty(newFolderPath))
                return;

            UnityAssetsRenamer.RenameAssetsInFolder(newFolderPath);

            CreatePrefabsInFolder(newFolderPath);

            Debug.Log($"Materiales, prefab y assets listos en {newFolderPath}.");
        }
    }
}
