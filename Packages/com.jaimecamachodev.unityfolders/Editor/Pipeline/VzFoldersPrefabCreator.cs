#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static VzFolders.Libs.VUtils;

namespace VzFolders.Pipeline
{
    // Instantiates every model found in <folder>/Mesh and saves it as a prefab in <folder>/Prefab.
    static class VzFoldersPrefabCreator
    {
        const string dir = "Tools/JaimeCamachoDev/VzFolders/";
        const string assetsDir = "Assets/VzFolders/";

        [MenuItem(dir + "Ingest pipeline/Create prefabs from Mesh folder", false, 242)]
        [MenuItem(assetsDir + "Ingest pipeline/Create prefabs from Mesh folder", false, 242)]
        static void CreatePrefabsMenu()
        {
            var folderPath = VzFoldersPipelineUtils.GetSelectedFolderOrFallback();
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning("VzFolders: selecciona una carpeta válida para crear prefabs.");
                return;
            }

            CreatePrefabsInFolder(folderPath);
        }

        internal static void CreatePrefabsInFolder(string folderPath)
        {
            AssetDatabase.Refresh(); // import any model dropped in via the OS file explorer before AssetDatabase can see it

            var meshFolder = folderPath.CombinePath(VzFoldersAssetTypeFolders.Mesh);
            var prefabFolder = folderPath.CombinePath(VzFoldersAssetTypeFolders.Prefab);

            if (!AssetDatabase.IsValidFolder(meshFolder))
            {
                Debug.LogWarning($"VzFolders: no existe la carpeta {meshFolder}, nada que convertir a prefab.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(prefabFolder))
            {
                AssetDatabase.CreateFolder(folderPath, VzFoldersAssetTypeFolders.Prefab);
                AssetDatabase.Refresh(); // the new Prefab folder must exist on disk before we generate a unique path into it below
            }

            var modelGuids = AssetDatabase.FindAssets("t:Model", new[] { meshFolder });
            if (modelGuids.Length == 0)
            {
                Debug.LogWarning($"VzFolders: no se encontró ninguna malla en {meshFolder} — no se creó ningún prefab.");
                return;
            }

            foreach (var guid in modelGuids)
            {
                var modelPath = guid.ToPath();
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (model == null) continue;

                var prefix = model.name.EndsWith("_Geo") ? model.name.Substring(0, model.name.Length - 4) : model.name;
                var prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabFolder.CombinePath(prefix + ".prefab"));

                var instance = PrefabUtility.InstantiatePrefab(model) as GameObject;
                instance.name = prefix;
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                Object.DestroyImmediate(instance);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"VzFolders: prefabs creados en {prefabFolder}.");
        }
    }
}
#endif
