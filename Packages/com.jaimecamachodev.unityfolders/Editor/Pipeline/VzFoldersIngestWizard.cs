#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VzFolders.Pipeline
{
    // One configurable wizard covering the full raw-asset-dump ingest flow so the
    // "create materials -> wrap in folder -> rename -> create prefabs" steps aren't
    // duplicated across several near-identical menu commands.
    static class VzFoldersIngestWizard
    {
        const string dir = "Tools/JaimeCamachoDev/VzFolders/";
        const string assetsDir = "Assets/VzFolders/";

        enum MaterialMode { None, Mas, Lit }

        [MenuItem(dir + "Ingest pipeline/Full pipeline...", false, 243)]
        [MenuItem(assetsDir + "Full ingest pipeline...", false, 243)]
        static void ShowWizard()
        {
            var folderPath = VzFoldersPipelineUtils.GetSelectedFolderOrFallback();
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning("VzFolders: selecciona una carpeta válida.");
                return;
            }

            IngestWizard.Show(folderPath);
        }

        class IngestWizard : ScriptableWizard
        {
            public MaterialMode createMaterials = MaterialMode.None;
            public bool wrapInNewSubfolder = true;
            public string newSubfolderName = "New Folder";
            public bool renameAssetsWithPrefix = true;
            public bool createPrefabsFromMeshes = false;

            string folderPath;

            public static void Show(string folderPath)
            {
                var wizard = DisplayWizard<IngestWizard>("VzFolders — Full ingest pipeline", "Run", "Cancel");
                wizard.folderPath = folderPath;
            }

            void OnWizardCreate()
            {
                var workingFolder = folderPath;

                switch (createMaterials)
                {
                    case MaterialMode.Mas: VzFoldersMaterialsCreator.CreateMasMaterialsInFolder(workingFolder); break;
                    case MaterialMode.Lit: VzFoldersMaterialsCreator.CreateLitMaterialsInFolder(workingFolder); break;
                }

                if (wrapInNewSubfolder)
                {
                    var newFolder = VzFoldersOrganizer.OrganizeIntoNewSubfolder(workingFolder, newSubfolderName);
                    if (string.IsNullOrEmpty(newFolder)) return;
                    workingFolder = newFolder;
                }
                else
                {
                    VzFoldersOrganizer.OrganizeFolder(workingFolder);
                }

                if (renameAssetsWithPrefix)
                    VzFoldersAssetsRenamer.RenameAssetsInFolder(workingFolder);

                if (createPrefabsFromMeshes)
                    VzFoldersPrefabCreator.CreatePrefabsInFolder(workingFolder);

                Debug.Log($"VzFolders: pipeline de ingesta completado en {workingFolder}.");
            }
        }
    }
}
#endif
