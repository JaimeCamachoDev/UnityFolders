using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Borodar.RainbowFolders
{
    public static class ProjectPreferences
    {
        public static bool ShowProjectTree => true;
        public static bool DrawRowShading => false;
        public static string RulesetPath => "Assets/Resources/UnityFolders/FolderRuleset.asset";

        public static bool IsEditModifierPressed(Event e) => false;
        public static bool IsDragModifierPressed(Event e) => false;

        [SettingsProvider]
        public static SettingsProvider CreateSettingProvider()
        {
            return new SettingsProvider("Borodar/RainbowFolders", SettingsScope.Project)
            {
                label = "RainbowFolders",
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.HelpBox("RainbowFolders Settings (reducido).", MessageType.Info);
                    EditorGUILayout.LabelField("Ruleset Path", RulesetPath);
                }
            };
        }

        public static void UpdateRulesetPath(string path, bool updatePref, bool reloadInstance)
        {
            // noop
        }
    }
}
