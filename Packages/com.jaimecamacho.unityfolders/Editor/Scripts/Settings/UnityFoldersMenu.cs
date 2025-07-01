
using UnityEditor;
using UnityEngine;
using Borodar.RainbowFolders;

namespace JaimeCamachoDev.UnityFolders
{
    public static class UnityFoldersMenu
    {
        private const string RULESET_NAME = "FolderRuleset.asset";
        private const string RULESET_PATH = "Assets/Resources/UnityFolders/";

        [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Create Ruleset")]
        public static void CreateRuleset()
        {
            var ruleset = ScriptableObject.CreateInstance<ProjectRuleset>();
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            if (!AssetDatabase.IsValidFolder("Assets/Resources/UnityFolders"))
                AssetDatabase.CreateFolder("Assets/Resources", "UnityFolders");

            AssetDatabase.CreateAsset(ruleset, RULESET_PATH + RULESET_NAME);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = ruleset;
        }
    }
}