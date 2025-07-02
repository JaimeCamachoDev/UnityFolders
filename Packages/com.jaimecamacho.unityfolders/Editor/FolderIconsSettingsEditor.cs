using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using JaimeCamachoDev.UnityFolders;

[CustomEditor(typeof(FolderIconsSettings))]
public class FolderIconsSettingsEditor : Editor
{
    private ReorderableList list;
    private FolderIconsSettings settings;

    private void OnEnable()
    {
        settings = (FolderIconsSettings)target;

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
                serializedObject.ApplyModifiedProperties();

                var rule = settings.rules[index];
                if (rule.iconSmall != null)
                {
                    var newSmall = FolderIconRecolorUtility.RecolorAndSave(rule.iconSmall, rule.background, rule.match + "_Small");
                    rule.iconSmall = newSmall;
                }

                if (rule.iconLarge != null)
                {
                    var newLarge = FolderIconRecolorUtility.RecolorAndSave(rule.iconLarge, rule.background, rule.match + "_Large");
                    rule.iconLarge = newLarge;
                }

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
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