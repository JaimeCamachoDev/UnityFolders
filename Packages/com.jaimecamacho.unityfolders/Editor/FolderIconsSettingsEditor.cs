using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using JaimeCamachoDev.UnityFolders;

[CustomEditor(typeof(FolderIconsSettings))]
public class FolderIconsSettingsEditor : Editor
{
    private ReorderableList list;
    private readonly Dictionary<Color, Texture2D> gradientCache = new();

    private Texture2D GetGradient(Color baseColor)
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

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject,
                                    serializedObject.FindProperty("rules"),
                                    draggable: true, displayHeader: true,
                                    displayAddButton: true, displayRemoveButton: true);

        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Folder Icon Rules");
        };

        list.elementHeightCallback = index =>
        {
            float h = EditorGUIUtility.singleLineHeight + 4;
            float preview = h * 2.5f;
            return h * 7 + preview + 8; // fields + button + margin
        };

        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            float lineHeight = EditorGUIUtility.singleLineHeight + 4;
            float y = rect.y;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("match"), new GUIContent("Match Path"));
            y += lineHeight;
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("matchType"), new GUIContent("Match Type"));
            y += lineHeight;
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("background"), new GUIContent("Tint Color"));
            y += lineHeight;
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("iconSmall"), new GUIContent("Icon Small"));
            y += lineHeight;
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("iconLarge"), new GUIContent("Icon Large"));
            y += lineHeight;
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("overlayIcon"), new GUIContent("Overlay Icon"));
            y += lineHeight;

            // Previews
            var settings = (FolderIconsSettings)target;
            var rule = settings.rules[index];
            float previewSize = lineHeight * 2.5f;
            Rect smallRect = new Rect(rect.x, y, previewSize, previewSize);
            Rect largeRect = new Rect(rect.x + previewSize + 4, y, previewSize, previewSize);

            if (rule.iconSmall != null)
            {
                if (rule.background.a > 0.01f)
                    GUI.DrawTexture(smallRect, GetGradient(rule.background), ScaleMode.StretchToFill);
                var tex = FolderIconRecolorUtility.Recolor(rule.iconSmall, rule.background);
                GUI.DrawTexture(smallRect, tex, ScaleMode.ScaleToFit, true);
            }

            if (rule.iconLarge != null)
            {
                if (rule.background.a > 0.01f)
                    GUI.DrawTexture(largeRect, GetGradient(rule.background), ScaleMode.StretchToFill);
                var tex = FolderIconRecolorUtility.Recolor(rule.iconLarge, rule.background, rule.overlayIcon);
                GUI.DrawTexture(largeRect, tex, ScaleMode.ScaleToFit, true);
            }

            y += previewSize + 4;

            if (GUI.Button(new Rect(rect.x, y, rect.width, lineHeight), "Apply Color & Generate Icons"))
            {
                if (rule.iconSmall != null)
                    rule.iconSmall = FolderIconRecolorUtility.RecolorAndSave(rule.iconSmall, rule.background, rule.match + "_Small");
                if (rule.iconLarge != null)
                    rule.iconLarge = FolderIconRecolorUtility.RecolorAndSave(rule.iconLarge, rule.background, rule.match + "_Large", rule.overlayIcon);
            }

            // Keep height in sync with elementHeightCallback
            y += lineHeight + 4;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}