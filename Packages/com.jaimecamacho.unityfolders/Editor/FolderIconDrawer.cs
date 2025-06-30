using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[InitializeOnLoad]
public static class FolderIconDrawer
{
    private static FolderIconSettings settings;

    static FolderIconDrawer()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        LoadSettings();
    }

    private static void LoadSettings()
    {
        if (settings != null) return;

        string[] guids = AssetDatabase.FindAssets("t:FolderIconSettings");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            settings = AssetDatabase.LoadAssetAtPath<FolderIconSettings>(path);
        }
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        if (settings == null) return;

        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!AssetDatabase.IsValidFolder(path)) return;

        string folderName = Path.GetFileName(path);

        foreach (var rule in settings.rules.Where(r => r.enabled))
        {
            if (!MatchRule(rule, path, folderName)) continue;

            //  FONDO GENERAL (fondo de la línea)
            if (rule.color.a > 0.01f)
            {
                Color oldColor = GUI.color;
                GUI.color = rule.color;

                Rect backgroundRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.height);
                GUI.DrawTexture(backgroundRect, Texture2D.whiteTexture);

                GUI.color = oldColor;
            }

            //  FONDO DEL NOMBRE (zona derecha del texto de la carpeta)
            if (rule.color.a > 0.01f)
            {
                var textRect = new Rect(selectionRect.x + 18f, selectionRect.y, selectionRect.width - 18f, selectionRect.height);
                EditorGUI.DrawRect(textRect, rule.color);
            }

            //  ICONO PERSONALIZADO
            if (rule.icon != null)
            {
                Rect iconRect = new Rect(selectionRect.x, selectionRect.y, 16, 16);
                GUI.DrawTexture(iconRect, rule.icon);
            }

            break; // solo aplicar la primera coincidencia
        }
    }

    private static bool MatchRule(FolderIconRule rule, string fullPath, string folderName)
    {
        switch (rule.ruleType)
        {
            case RuleType.Name:
                return folderName.Equals(rule.match, System.StringComparison.OrdinalIgnoreCase);
            case RuleType.Path:
                return fullPath.Contains(rule.match);
            case RuleType.Regex:
                return System.Text.RegularExpressions.Regex.IsMatch(fullPath, rule.match);
            default:
                return false;
        }
    }
}
