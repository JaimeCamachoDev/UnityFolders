using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders
{
    public class ProjectBackgroundsPopup : EditorWindow
    {
        private Vector2 scroll;

        [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Show Backgrounds Popup")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectBackgroundsPopup>(true, "Backgrounds Popup");
            window.minSize = new Vector2(300, 200);
        }

        public static T GetDraggableWindow<T>() where T : EditorWindow
        {
            return EditorWindow.GetWindow<T>(true, typeof(T).Name, true);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Backgrounds Popup (Simplificado)", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.HelpBox("Aquí podrías mostrar fondos si decides mantener esta vista.", MessageType.Info);
            EditorGUILayout.EndScrollView();
        }
    }
}