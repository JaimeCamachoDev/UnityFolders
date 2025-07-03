using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using JaimeCamachoDev.UnityFolders;

[CustomEditor(typeof(FolderIconsSettings))]
public class FolderIconsSettingsEditor : Editor
{
    private ReorderableList list;

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

        list.elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 7 + 36;

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

            if (GUI.Button(new Rect(rect.x, y, rect.width, lineHeight), "Apply Color & Generate Icons"))
            {
                var settings = (FolderIconsSettings)target;
                var rule = settings.rules[index];
                if (rule.iconSmall != null)
                    rule.iconSmall = FolderIconRecolorUtility.RecolorAndSave(rule.iconSmall, rule.background, rule.match + "_Small", rule.overlayIcon);
                if (rule.iconLarge != null)
                    rule.iconLarge = FolderIconRecolorUtility.RecolorAndSave(rule.iconLarge, rule.background, rule.match + "_Large", rule.overlayIcon);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}