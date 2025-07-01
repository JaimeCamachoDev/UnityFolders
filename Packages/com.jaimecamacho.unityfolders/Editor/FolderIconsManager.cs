using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
[InitializeOnLoad]
public static class FolderIconsManager
{
    static FolderIconsSettings settings;

    static FolderIconsManager()
    {
        EditorApplication.projectWindowItemOnGUI += DrawCustomIcons;
        LoadSettings();
    }

    static void LoadSettings()
    {
        var guid = AssetDatabase.FindAssets("t:FolderIconsSettings").FirstOrDefault();
        if (guid != null)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            settings = AssetDatabase.LoadAssetAtPath<FolderIconsSettings>(path);
        }
    }

    static void DrawCustomIcons(string guid, Rect rect)
    {
        if (settings == null) return;

        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!AssetDatabase.IsValidFolder(path)) return;

        string folderName = Path.GetFileName(path);

        foreach (var rule in settings.rules.Where(r => r.enabled).OrderByDescending(r => r.priority))
        {
            if (!Match(rule, path, folderName)) continue;

            Rect iconRect = new Rect(rect.x, rect.y, rect.height, rect.height);

            if (rule.eraseDefault && rule.background.a > 0.01f)
                EditorGUI.DrawRect(iconRect, rule.background);

            Texture2D icon = iconRect.height > 32 ? rule.iconLarge : rule.iconSmall;
            if (icon != null)
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);

            break;
        }
    }

    static bool Match(FolderIconRule rule, string path, string name)
    {
        return rule.matchType switch
        {
            MatchType.Name => name.Equals(rule.match, StringComparison.OrdinalIgnoreCase),
            MatchType.Path => path.Replace("\\", "/").Contains(rule.match),
            MatchType.Regex => System.Text.RegularExpressions.Regex.IsMatch(path, rule.match),
            _ => false,
        };
    }
}
