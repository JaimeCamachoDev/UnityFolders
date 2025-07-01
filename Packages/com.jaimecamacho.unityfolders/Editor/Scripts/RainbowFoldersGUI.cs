using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders
{
    [InitializeOnLoad]
    public static class RainbowFoldersGUI
    {
        static RainbowFoldersGUI()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            // Aquí podrías aplicar tu lógica personalizada de fondo/icono
            // Para esta versión simplificada solo se mantiene estable la estructura
        }

        [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Show Main Popup")]
        public static void ShowPopup()
        {
            ProjectPopupWindow.ShowWindow();
        }
    }
}