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
            float lineHeight = EditorGUIUtility.singleLineHeight + 4;
            float height = lineHeight * 7;
            if (((FolderIconsSettings)target).showPreview)
                height += lineHeight * 2.5f + 4;
            return height;
        };

        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            float lineHeight = EditorGUIUtility.singleLineHeight + 4;
            float y = rect.y;

            var backgroundProp = element.FindPropertyRelative("background");
            var iconSmallProp = element.FindPropertyRelative("iconSmall");
            var iconLargeProp = element.FindPropertyRelative("iconLarge");
            var overlayProp = element.FindPropertyRelative("overlayIcon");
            var previewSmallProp = element.FindPropertyRelative("previewSmall");
            var previewLargeProp = element.FindPropertyRelative("previewLarge");

            bool updatePreview = false;

            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("match"), new GUIContent("Match Path"));
            y += lineHeight;
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), element.FindPropertyRelative("matchType"), new GUIContent("Match Type"));
            y += lineHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), backgroundProp, new GUIContent("Tint Color"));
            if (EditorGUI.EndChangeCheck()) updatePreview = true;
            y += lineHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), iconSmallProp, new GUIContent("Icon Small"));
            if (EditorGUI.EndChangeCheck()) updatePreview = true;
            y += lineHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), iconLargeProp, new GUIContent("Icon Large"));
            if (EditorGUI.EndChangeCheck()) updatePreview = true;
            y += lineHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), overlayProp, new GUIContent("Overlay Icon"));
            if (EditorGUI.EndChangeCheck()) updatePreview = true;
            y += lineHeight;

            if (updatePreview)
            {
                Texture2D small = iconSmallProp.objectReferenceValue as Texture2D;
                Texture2D large = iconLargeProp.objectReferenceValue as Texture2D;
                Texture2D overlay = overlayProp.objectReferenceValue as Texture2D;
                Color bg = backgroundProp.colorValue;

                previewSmallProp.objectReferenceValue = small != null ? FolderIconRecolorUtility.Recolor(small, bg) : null;
                previewLargeProp.objectReferenceValue = large != null ? FolderIconRecolorUtility.Recolor(large, bg, overlay) : null;
            }

            var settings = (FolderIconsSettings)target;
            if (settings.showPreview)
            {
                float previewSize = lineHeight * 2.5f;
                Rect smallRect = new Rect(rect.x, y, previewSize, previewSize);
                Rect largeRect = new Rect(rect.x + previewSize + 4, y, previewSize, previewSize);

                Texture2D smallTex = previewSmallProp.objectReferenceValue as Texture2D;
                Texture2D largeTex = previewLargeProp.objectReferenceValue as Texture2D;

                if (smallTex != null)
                {
                    if (backgroundProp.colorValue.a > 0.01f)
                        GUI.DrawTexture(smallRect, GetGradient(backgroundProp.colorValue), ScaleMode.StretchToFill);
                    GUI.DrawTexture(smallRect, smallTex, ScaleMode.ScaleToFit, true);
                }

                if (largeTex != null)
                {
                    if (backgroundProp.colorValue.a > 0.01f)
                        GUI.DrawTexture(largeRect, GetGradient(backgroundProp.colorValue), ScaleMode.StretchToFill);
                    GUI.DrawTexture(largeRect, largeTex, ScaleMode.ScaleToFit, true);
                }

                y += previewSize + 4;
            }

            if (GUI.Button(new Rect(rect.x, y, rect.width, lineHeight), "Apply Color & Generate Icons"))
            {
                if (iconSmallProp.objectReferenceValue != null)
                    iconSmallProp.objectReferenceValue = FolderIconRecolorUtility.RecolorAndSave((Texture2D)iconSmallProp.objectReferenceValue, backgroundProp.colorValue, element.FindPropertyRelative("match").stringValue + "_Small");
                if (iconLargeProp.objectReferenceValue != null)
                    iconLargeProp.objectReferenceValue = FolderIconRecolorUtility.RecolorAndSave((Texture2D)iconLargeProp.objectReferenceValue, backgroundProp.colorValue, element.FindPropertyRelative("match").stringValue + "_Large", overlayProp.objectReferenceValue as Texture2D);

                previewSmallProp.objectReferenceValue = iconSmallProp.objectReferenceValue as Texture2D;
                previewLargeProp.objectReferenceValue = iconLargeProp.objectReferenceValue as Texture2D;
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showPreview"));
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
