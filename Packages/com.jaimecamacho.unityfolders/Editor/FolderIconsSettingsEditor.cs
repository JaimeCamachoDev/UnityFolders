
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FolderIconsSettings))]
public class FolderIconsSettingsEditor : Editor
{
    private SerializedProperty rules;

    private void OnEnable()
    {
        rules = serializedObject.FindProperty("rules");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Reglas de Carpetas", EditorStyles.boldLabel);

        for (int i = 0; i < rules.arraySize; i++)
        {
            SerializedProperty rule = rules.GetArrayElementAtIndex(i);
            SerializedProperty name = rule.FindPropertyRelative("match");
            SerializedProperty type = rule.FindPropertyRelative("matchType");
            SerializedProperty iconSmall = rule.FindPropertyRelative("iconSmall");
            SerializedProperty iconLarge = rule.FindPropertyRelative("iconLarge");
            SerializedProperty background = rule.FindPropertyRelative("background");
            SerializedProperty enabled = rule.FindPropertyRelative("enabled");
            SerializedProperty priority = rule.FindPropertyRelative("priority");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            name.stringValue = EditorGUILayout.TextField(name.stringValue);
            type.enumValueIndex = (int)(MatchType)EditorGUILayout.EnumPopup((MatchType)type.enumValueIndex);
            enabled.boolValue = EditorGUILayout.Toggle(enabled.boolValue, GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();

            iconSmall.objectReferenceValue = EditorGUILayout.ObjectField("Icon Small", iconSmall.objectReferenceValue, typeof(Texture2D), false) as Texture2D;
            iconLarge.objectReferenceValue = EditorGUILayout.ObjectField("Icon Large", iconLarge.objectReferenceValue, typeof(Texture2D), false) as Texture2D;
            background.colorValue = EditorGUILayout.ColorField("Background", background.colorValue);
            priority.intValue = EditorGUILayout.IntField("Priority", priority.intValue);

            // Preview
            Rect previewRect = GUILayoutUtility.GetRect(64, 20, GUILayout.ExpandWidth(true));
            DrawPreview(previewRect, background.colorValue, iconSmall.objectReferenceValue as Texture2D, name.stringValue);

            if (GUILayout.Button("Eliminar esta regla"))
                rules.DeleteArrayElementAtIndex(i);

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Añadir nueva regla"))
        {
            rules.InsertArrayElementAtIndex(rules.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPreview(Rect rect, Color background, Texture2D icon, string text)
    {
        Color prevColor = GUI.color;
        GUI.color = background;
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
        GUI.color = prevColor;

        if (icon != null)
        {
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.height, rect.height), icon, ScaleMode.ScaleToFit, true);
        }

        Rect labelRect = new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height);
        GUI.Label(labelRect, text, EditorStyles.whiteLabel);
    }
}
