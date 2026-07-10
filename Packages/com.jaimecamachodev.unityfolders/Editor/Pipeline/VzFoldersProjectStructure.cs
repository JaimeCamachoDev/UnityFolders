#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static VzFolders.Libs.VUtils;

namespace VzFolders.Pipeline
{
    static class VzFoldersProjectStructure
    {
        const string dir = "Tools/JaimeCamachoDev/VzFolders/";
        const string assetsDir = "Assets/VzFolders/";

        [MenuItem(dir + "Project structure/Create base project structure", false, 200)]
        [MenuItem(assetsDir + "Create base project structure", false, 200)]
        static void CreateBaseProjectStructure()
        {
            CreateFolder("Assets", "1-Programming");
            CreateFolder("Assets/1-Programming", "Observers");
            CreateFolder("Assets/1-Programming", "Prefabs");
            CreateFolder("Assets/1-Programming", "Scripts");
            CreateFolder("Assets/1-Programming", "SolidMats");

            CreateFolder("Assets", "2-Art");
            CreateFolder("Assets/2-Art", "1-3D");
            CreateFolder("Assets/2-Art/1-3D", "Animals");
            CreateFolder("Assets/2-Art/1-3D", "Characters");
            CreateFolder("Assets/2-Art/1-3D", "Environments");
            CreateFolder("Assets/2-Art", "2-VFX");
            CreateFolder("Assets/2-Art", "3-SFX");
            CreateFolder("Assets/2-Art", "4-Directors");
            CreateFolder("Assets/2-Art", "5-Skyboxes");
            CreateFolder("Assets/2-Art", "6-Videos");
            CreateFolder("Assets/2-Art", "7-PostProcessing");
            CreateFolder("Assets/2-Art", "8-UI");

            CreateFolder("Assets", "3-Scenes");
            CreateFolder("Assets", "4-Presets");
            CreateFolder("Assets", "5-Settings");

            AssetDatabase.Refresh();
            Debug.Log("VzFolders: estructura base del proyecto creada.");
        }

        [MenuItem(dir + "Project structure/Create asset type folders...", false, 201)]
        [MenuItem(assetsDir + "Create asset type folders...", false, 201)]
        static void CreateAssetTypeFoldersMenu()
        {
            NewTypedFolderWizard.Show(VzFoldersPipelineUtils.GetSelectedFolderOrFallback());
        }

        // Creates <parentPath>/<name>/{Animation,Audio,Material,Mesh,Prefab,Script,Shader,VFX}
        internal static string CreateAssetTypeFolders(string parentPath, string name)
        {
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(parentPath.CombinePath(name));
            AssetDatabase.CreateFolder(parentPath, uniquePath.GetFilename(withExtension: true));

            foreach (var subfolder in VzFoldersAssetTypeFolders.All)
                CreateFolder(uniquePath, subfolder);

            AssetDatabase.Refresh();
            return uniquePath;
        }

        static void CreateFolder(string parentPath, string folderName)
        {
            var fullPath = parentPath.CombinePath(folderName);
            if (!AssetDatabase.IsValidFolder(fullPath))
                AssetDatabase.CreateFolder(parentPath, folderName);
        }

        class NewTypedFolderWizard : ScriptableWizard
        {
            public string folderName = "New Folder";
            string parentPath;

            public static void Show(string parentPath)
            {
                var wizard = DisplayWizard<NewTypedFolderWizard>("Create asset type folders", "Create", "Cancel");
                wizard.parentPath = parentPath;
            }

            void OnWizardCreate()
            {
                if (string.IsNullOrWhiteSpace(folderName))
                {
                    Debug.LogWarning("VzFolders: introduce un nombre de carpeta válido.");
                    return;
                }

                var createdPath = CreateAssetTypeFolders(parentPath, folderName);
                Debug.Log($"VzFolders: carpetas de tipo de asset creadas en {createdPath}.");
            }
        }
    }
}
#endif
