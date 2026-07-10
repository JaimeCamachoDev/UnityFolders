#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using static VzFolders.Libs.VUtils;

namespace VzFolders.Pipeline
{
    // Sorts a folder where artists dropped materials/meshes/textures/etc. all mixed together
    // into the shared Animation/Audio/Material/Mesh/Prefab/Script/Shader/VFX taxonomy.
    static class VzFoldersOrganizer
    {
        const string dir = "Tools/JaimeCamachoDev/VzFolders/";
        const string assetsDir = "Assets/VzFolders/";

        [MenuItem(dir + "Organize/Organize this folder", false, 220)]
        [MenuItem(assetsDir + "Organize this folder", false, 220)]
        static void OrganizeSelectedFolder()
        {
            var folderPath = VzFoldersPipelineUtils.GetSelectedFolderOrFallback();
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning("VzFolders: selecciona una carpeta válida para ordenar.");
                return;
            }

            OrganizeFolder(folderPath);
        }

        [MenuItem(dir + "Organize/Organize into new subfolder...", false, 221)]
        [MenuItem(assetsDir + "Organize into new subfolder...", false, 221)]
        static void OrganizeIntoNewSubfolderMenu()
        {
            var folderPath = VzFoldersPipelineUtils.GetSelectedFolderOrFallback();
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning("VzFolders: selecciona una carpeta válida para ordenar.");
                return;
            }

            NewSubfolderWizard.Show(folderPath);
        }

        // Wraps everything currently inside folderPath into a new subfolder named newFolderName, then organizes it.
        // Returns the new subfolder's path, or null if the name was invalid.
        internal static string OrganizeIntoNewSubfolder(string folderPath, string newFolderName)
        {
            if (string.IsNullOrWhiteSpace(newFolderName))
            {
                Debug.LogWarning("VzFolders: introduce un nombre válido para la nueva carpeta.");
                return null;
            }

            var newFolderPath = AssetDatabase.GenerateUniqueAssetPath(folderPath.CombinePath(newFolderName));
            AssetDatabase.CreateFolder(folderPath, newFolderPath.GetFilename(withExtension: true));

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (var file in Directory.GetFiles(folderPath))
                {
                    if (file.EndsWith(".meta")) continue;
                    var destination = AssetDatabase.GenerateUniqueAssetPath(newFolderPath.CombinePath(Path.GetFileName(file)));
                    AssetDatabase.MoveAsset(file.Replace("\\", "/"), destination);
                }

                foreach (var directory in Directory.GetDirectories(folderPath))
                {
                    var normalized = directory.Replace("\\", "/");
                    if (normalized == newFolderPath) continue;

                    var destination = AssetDatabase.GenerateUniqueAssetPath(newFolderPath.CombinePath(Path.GetFileName(directory)));
                    AssetDatabase.MoveAsset(normalized, destination);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();
            OrganizeFolder(newFolderPath);
            return newFolderPath;
        }

        // Moves every loose asset directly inside folderPath into its type subfolder (created on demand),
        // then deletes any subfolder left empty by the move.
        internal static void OrganizeFolder(string folderPath)
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                var assetGuids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
                foreach (var guid in assetGuids)
                {
                    var assetPath = guid.ToPath();
                    if (AssetDatabase.IsValidFolder(assetPath)) continue;

                    var destinationFolderName = VzFoldersAssetTypeFolders.GetFolderForAsset(assetPath);
                    if (destinationFolderName == null) continue;

                    var destinationFolder = folderPath.CombinePath(destinationFolderName);
                    if (!AssetDatabase.IsValidFolder(destinationFolder))
                        AssetDatabase.CreateFolder(folderPath, destinationFolderName);

                    var destinationPath = AssetDatabase.GenerateUniqueAssetPath(destinationFolder.CombinePath(assetPath.GetFilename(withExtension: true)));
                    AssetDatabase.MoveAsset(assetPath, destinationPath);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            VzFoldersPipelineUtils.RemoveEmptySubdirectories(folderPath);
            AssetDatabase.Refresh();
            Debug.Log($"VzFolders: carpeta {folderPath} ordenada.");
        }

        class NewSubfolderWizard : ScriptableWizard
        {
            public string newFolderName = "New Folder";
            string folderPath;

            public static void Show(string folderPath)
            {
                var wizard = DisplayWizard<NewSubfolderWizard>("Organize into new subfolder", "Organize", "Cancel");
                wizard.folderPath = folderPath;
            }

            void OnWizardCreate() => OrganizeIntoNewSubfolder(folderPath, newFolderName);
        }
    }
}
#endif
