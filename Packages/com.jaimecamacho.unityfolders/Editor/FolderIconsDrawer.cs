using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JaimeCamachoDev.UnityFolders
{
    [InitializeOnLoad]
    public static class FolderIconsDrawer
    {
        static FolderIconsSettings settings;
        static Dictionary<Color, Texture2D> gradientCache = new();

        static FolderIconsDrawer()
        {
            settings = Resources.Load<FolderIconsSettings>("UnityFolders/FolderIconsSettings");

            if (settings == null) return;

            EditorApplication.projectWindowItemOnGUI += OnItemGUI;
        }

        private static void OnItemGUI(string guid, Rect rect)
        {
            if (settings == null || settings.rules == null) return;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path)) return;

            foreach (var rule in settings.rules)
            {
                if (string.IsNullOrEmpty(rule.match) || !path.Contains(rule.match))
                    continue;

                // Draw background gradient
                if (rule.background.a > 0.01f)
                    GUI.DrawTexture(rect, GetGradient(rule.background), ScaleMode.StretchToFill);

                // Replace icon
                Texture2D icon = rule.iconSmall;
                if (icon == null) break;

                float iconSize = rect.height;
                Rect iconRect = new Rect(rect.x, rect.y, iconSize, iconSize);
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
                break;
            }
        }

        private static Texture2D GetGradient(Color baseColor)
        {
            if (gradientCache.TryGetValue(baseColor, out var cached))
                return cached;

            var top = baseColor;
            var bottom = Color.Lerp(baseColor, Color.black, 0.25f);

            Texture2D tex = new Texture2D(1, 2) { wrapMode = TextureWrapMode.Clamp };
            tex.SetPixels(new[] { top, bottom });
            tex.Apply();

            gradientCache[baseColor] = tex;
            return tex;
        }
    }
}