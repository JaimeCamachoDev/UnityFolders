using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders
{
    public class ProjectIconsPopup : EditorWindow
    {
        private Vector2 scroll;

        [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Show Icons Popup")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectIconsPopup>(true, "Icons Popup");
            window.minSize = new Vector2(300, 200);
        }

        public static T GetDraggableWindow<T>() where T : EditorWindow
        {
            return EditorWindow.GetWindow<T>(true, typeof(T).Name, true);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Icons Popup (Simplificado)", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.HelpBox("Aquí podrías mostrar iconos si decides hacerlo más adelante.", MessageType.Info);
            EditorGUILayout.EndScrollView();
        }
    }
}