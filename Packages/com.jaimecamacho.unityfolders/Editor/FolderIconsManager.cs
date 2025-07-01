
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

[InitializeOnLoad]
public static class FolderIconsManager
{
    static FolderIconsSettings settings;
    static Texture2D gradientTex;

    static FolderIconsManager()
    {
        EditorApplication.projectWindowItemOnGUI += DrawCustomIcons;
        LoadSettings();
        CreateGradient();
    }

    static void LoadSettings()
    {
        var guid = AssetDatabase.FindAssets("t:FolderIconsSettings").FirstOrDefault();
        if (!string.IsNullOrEmpty(guid))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            settings = AssetDatabase.LoadAssetAtPath<FolderIconsSettings>(path);
        }
    }

    static void CreateGradient()
    {
        gradientTex = new Texture2D(128, 1, TextureFormat.ARGB32, false);
        gradientTex.wrapMode = TextureWrapMode.Clamp;
        for (int x = 0; x < 128; x++)
        {
            float t = x / 127f;
            Color c = new Color(1, 1, 1, Mathf.Lerp(0.25f, 0f, t));
            gradientTex.SetPixel(x, 0, c);
        }
        gradientTex.Apply();
    }

    static void DrawCustomIcons(string guid, Rect selectionRect)
    {
        if (settings == null) return;

        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!AssetDatabase.IsValidFolder(path)) return;

        string folderName = Path.GetFileName(path);

        foreach (var rule in settings.rules.Where(r => r.enabled).OrderByDescending(r => r.priority))
        {
            if (!Match(rule, path, folderName)) continue;

            bool isGrid = selectionRect.height > 25f && selectionRect.width > 25f;
            Texture2D icon = isGrid ? rule.iconLarge : rule.iconSmall;

            // Gradiente en columna izquierda (solo sobre el ancho real del área de texto)
            if (rule.background.a > 0.01f)
            {
                Rect labelRect;
                if (isGrid)
                {
                    labelRect = new Rect(selectionRect.x, selectionRect.yMax - EditorGUIUtility.singleLineHeight + 2f, selectionRect.width, EditorGUIUtility.singleLineHeight);
                }
                else
                {
                    float xStart = selectionRect.x + selectionRect.height;
                    float width = selectionRect.width - (xStart - selectionRect.x);
                    width = Mathf.Max(48f, width);
                    labelRect = new Rect(xStart, selectionRect.y, width, selectionRect.height);
                }

                GUI.color = rule.background;
                GUI.DrawTexture(labelRect, gradientTex, ScaleMode.StretchToFill);
                GUI.color = Color.white;
            }

            // Icono centrado
            if (icon != null)
            {
                Rect iconRect = isGrid
                    ? new Rect(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.height - EditorGUIUtility.singleLineHeight)
                    : new Rect(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);

                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
            }

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
