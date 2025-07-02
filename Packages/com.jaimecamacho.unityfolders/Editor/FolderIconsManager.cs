using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Reflection;

[InitializeOnLoad]
public static class FolderIconsManager
{
    static FolderIconsSettings settings;
    static Texture2D gradientTex;
    static Type projectBrowserType;
    static FieldInfo lastInteractedProjectBrowserField;
    static FieldInfo viewModeField;

    static FolderIconsManager()
    {
        EditorApplication.projectWindowItemOnGUI += DrawCustomIcons;
        LoadSettings();
        CreateGradient();
        InitReflection();
    }

    static void InitReflection()
    {
        projectBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        lastInteractedProjectBrowserField = projectBrowserType?.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.NonPublic);
        viewModeField = projectBrowserType?.GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
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

    static bool IsInLeftColumn(Rect selectionRect)
    {
        if (projectBrowserType == null || lastInteractedProjectBrowserField == null || viewModeField == null)
            return false;

        var browser = lastInteractedProjectBrowserField.GetValue(null);
        if (browser == null) return false;

        int viewMode = (int)viewModeField.GetValue(browser);
        return viewMode == 1 && selectionRect.xMax < 200f && selectionRect.height <= EditorGUIUtility.singleLineHeight + 4;
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
            bool isInLeftColumn = !isGrid && IsInLeftColumn(selectionRect);
            Texture2D icon = isGrid ? rule.iconLarge : rule.iconSmall;

            // Gradiente
            if (rule.background.a > 0.01f)
            {
                Rect labelRect;
                if (isGrid)
                {
                    float textHeight = EditorGUIUtility.singleLineHeight;
                    float labelY = selectionRect.yMax - textHeight + 2f;
                    labelRect = new Rect(selectionRect.x, labelY, selectionRect.width, textHeight);
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

            // Icono ajustado según columna

            bool isSelected = Selection.activeObject != null && AssetDatabase.GetAssetPath(Selection.activeObject) == path;

            if (icon != null)
            {
                Rect iconRect;
                if (isGrid)
                {
                    float size = selectionRect.height - EditorGUIUtility.singleLineHeight;
                    float iconX = selectionRect.x + (selectionRect.width - size) * 0.5f;
                    iconRect = new Rect(iconX, selectionRect.y, size, size);
                }
                else if (isInLeftColumn)
                {
                    iconRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);
                }
                else
                {
                    iconRect = new Rect(selectionRect.x + 1, selectionRect.y, selectionRect.height, selectionRect.height);
                }

                // Truco para ocultar el ícono base de Unity sin cuadro blanco
                Color prevColor = GUI.color;
                GUI.color = new Color(0.2196f, 0.2196f, 0.2196f, 1f);
                GUI.DrawTexture(iconRect, Texture2D.whiteTexture);
                GUI.color = prevColor;

                // 🔵 Atenuar el icono si está seleccionada la carpeta
                prevColor = GUI.color;
                if (isSelected)
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);  // Semitransparente

                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
                GUI.color = prevColor;
            }
            break;
        }
    }

    static bool Match(FolderIconRule rule, string path, string name)
    {
        return rule.matchType switch
        {
            MatchType.Name => name.Equals(rule.match, StringComparison.OrdinalIgnoreCase),
            MatchType.Path => path.Replace("\\", " / ").Contains(rule.match),
            MatchType.Regex => System.Text.RegularExpressions.Regex.IsMatch(path, rule.match),
            _ => false,
        };
    }
}