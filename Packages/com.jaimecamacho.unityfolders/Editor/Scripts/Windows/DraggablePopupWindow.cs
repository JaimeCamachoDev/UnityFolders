using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders
{
    public class DraggablePopupWindow : EditorWindow
    {
        public static T GetDraggableWindow<T>() where T : EditorWindow
        {
            var window = GetWindow<T>(true, typeof(T).Name, true);
            window.minSize = new Vector2(300, 300);
            window.ShowUtility();
            return window;
        }
    }
}