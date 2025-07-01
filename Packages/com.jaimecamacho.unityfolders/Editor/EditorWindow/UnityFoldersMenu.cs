using UnityEditor;
using UnityEngine;

namespace UnityFolders
{
    public static class UnityFoldersMenu
    {
        [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Create Folder Ruleset")]
        public static void CreateRuleset()
        {
            var asset = ScriptableObject.CreateInstance<FolderRuleset>();
            string folderPath = "Assets/Resources/UnityFolders";
            string assetPath = $"{folderPath}/FolderRuleset.asset";

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets/Resources", "UnityFolders");

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }



        [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Create Folder Ruleset", false, 1000)]
        public static void CreateRulesetFromAssetsMenu() => CreateRuleset();


    }
}