using UnityEditor;
using UnityEngine;

namespace UnityFolders
{
    [InitializeOnLoad]
    public static class FolderIconsDrawer
    {
        static FolderIconsDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowGUI;
        }

        private static void OnProjectWindowGUI(string guid, Rect rect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path)) return;

            var ruleset = Resources.Load<FolderRuleset>("UnityFolders/FolderRuleset");
            if (ruleset == null) return;

            foreach (var rule in ruleset.rules)
            {
                bool match = rule.matchMode == MatchMode.NameContains && path.Contains(rule.matchValue) ||
                             rule.matchMode == MatchMode.PathContains && path.Replace("\\", "/").EndsWith(rule.matchValue);
                if (!match) continue;

                // Adjusted background rect to avoid overlap with selection
                Rect bgRect = new Rect(rect.x, rect.y, rect.width, rect.height);
                Texture2D bgTex = MakeHorizontalGradient(rule.backgroundColor, (int)bgRect.width, (int)bgRect.height);
                GUI.DrawTexture(bgRect, bgTex, ScaleMode.StretchToFill);

                // Icon
                if (rule.iconTexture != null)
                {
                    float iconSize = Mathf.Min(rect.height - 4f, 18f);
                    var iconRect = new Rect(rect.x + 2f, rect.y + (rect.height - iconSize) / 2f, iconSize, iconSize);
                    GUI.DrawTexture(iconRect, rule.iconTexture, ScaleMode.ScaleToFit, true);
                }

                break;
            }
        }

        private static Texture2D MakeHorizontalGradient(Color color, int width, int height)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int x = 0; x < width; x++)
            {
                float t = (float)x / width;
                Color c = Color.Lerp(color * 0.6f, color, t);
                for (int y = 0; y < height; y++)
                    tex.SetPixel(x, y, c);
            }

            tex.Apply();
            return tex;
        }
    }
}