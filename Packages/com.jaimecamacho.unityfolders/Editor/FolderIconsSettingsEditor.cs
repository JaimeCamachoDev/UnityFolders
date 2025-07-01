using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(FolderIconsSettings))]
public class FolderIconsSettingsEditor : Editor
{
    private ReorderableList list;

    private void OnEnable()
    {
        list = new ReorderableList(
            serializedObject,
            serializedObject.FindProperty("rules"),
            draggable: true,
            displayHeader: true,
            displayAddButton: true,
            displayRemoveButton: true
        );

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Reglas de Carpetas");
        };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            float y = rect.y + 2f;
            float x = rect.x;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(new Rect(x, y, rect.width * 0.5f, lineHeight), element.FindPropertyRelative("ruleName"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(x + rect.width * 0.52f, y, rect.width * 0.48f, lineHeight), element.FindPropertyRelative("matchType"), GUIContent.none);

            y += lineHeight + 2;
            EditorGUI.PropertyField(new Rect(x, y, rect.width, lineHeight), element.FindPropertyRelative("match"));

            y += lineHeight + 2;
            EditorGUI.PropertyField(new Rect(x, y, rect.width * 0.5f, lineHeight), element.FindPropertyRelative("iconSmall"));
            EditorGUI.PropertyField(new Rect(x + rect.width * 0.52f, y, rect.width * 0.48f, lineHeight), element.FindPropertyRelative("iconLarge"));

            y += lineHeight + 2;
            EditorGUI.PropertyField(new Rect(x, y, rect.width * 0.33f, lineHeight), element.FindPropertyRelative("background"));
            EditorGUI.PropertyField(new Rect(x + rect.width * 0.35f, y, rect.width * 0.3f, lineHeight), element.FindPropertyRelative("eraseDefault"));
            EditorGUI.PropertyField(new Rect(x + rect.width * 0.67f, y, rect.width * 0.33f, lineHeight), element.FindPropertyRelative("enabled"));

            y += lineHeight + 2;
            EditorGUI.PropertyField(new Rect(x, y, rect.width * 0.5f, lineHeight), element.FindPropertyRelative("priority"));
        };

        list.elementHeightCallback = (index) => EditorGUIUtility.singleLineHeight * 5 + 10;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
