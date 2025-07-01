using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders
{
    public class ProjectPopupWindow : EditorWindow
    {
        private Vector2 scroll;

        [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Popup Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectPopupWindow>(true, "Popup Window");
            window.minSize = new Vector2(300, 200);
        }

        public static T GetDraggableWindow<T>() where T : EditorWindow
        {
            return EditorWindow.GetWindow<T>(true, typeof(T).Name, true);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Project Popup Window (Simplificado)", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.HelpBox("Ventana simplificada. Aquí iría el contenido visual si decides activarlo más adelante.", MessageType.Info);
            EditorGUILayout.EndScrollView();
        }
    }
}