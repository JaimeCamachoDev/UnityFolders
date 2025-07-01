using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders
{
    [CustomPropertyDrawer(typeof(ProjectRule))]
    public class ProjectRuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var matchMode = property.FindPropertyRelative("MatchMode");
            var matchValue = property.FindPropertyRelative("MatchValue");

            var iconRect = new Rect(position.x, position.y, 20, position.height);
            var backgroundRect = new Rect(position.x + 25, position.y, 20, position.height);
            var modeRect = new Rect(position.x + 50, position.y, 100, position.height);
            var valueRect = new Rect(position.x + 155, position.y, position.width - 155, position.height);

            if (GUI.Button(iconRect, "I"))
            {
                ProjectIconsPopup.ShowWindow();
            }

            if (GUI.Button(backgroundRect, "B"))
            {
                ProjectBackgroundsPopup.ShowWindow();
            }

            EditorGUI.PropertyField(modeRect, matchMode, GUIContent.none);
            EditorGUI.PropertyField(valueRect, matchValue, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}