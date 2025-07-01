using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders
{
    public abstract class ProjectSelectionPopup<T> : DraggablePopupWindow
    {
        protected const float PREVIEW_SIZE_SMALL = 16f;
        protected const float PREVIEW_SIZE_LARGE = 28f;
        protected const float LINE_HEIGHT = 16f;
        protected const float SPACING = 4f;

        protected Vector2 ScrollPos;
        protected SerializedProperty ProjectRule;
        protected bool IsRuleSerialized;

        public void SetRule(object rule)
        {
            if (rule is SerializedProperty serializedProperty)
            {
                ProjectRule = serializedProperty;
                IsRuleSerialized = true;
            }
            else
            {
                ProjectRule = null;
                IsRuleSerialized = false;
            }
        }

        protected void ApplyPropertyChangesAndClose(SerializedProperty prop)
        {
            prop.serializedObject.ApplyModifiedProperties();
            CloseAndRepaintParent();
        }

        protected void CloseAndRepaintParent()
        {
            Close();
            GUIUtility.ExitGUI();
        }

        protected Vector2 BeginScrollView(Vector2 scroll)
        {
            return EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(position.width));
        }

        protected void DrawIconsGrid(List<T> items)
        {
            GUILayout.BeginHorizontal();
            int count = 0;
            foreach (var item in items)
            {
                DrawIconButton(item);
                count++;
                if (count % 6 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }

        protected abstract void DrawButtons(Rect rect);
        protected abstract void DrawIcons(Rect rect);
        protected abstract void DrawIconButton(T item);

        protected T GetSelectedEnum(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public new static W GetDraggableWindow<W>() where W : EditorWindow
        {
            var window = GetWindow<W>(true);
            window.minSize = new Vector2(300, 300);
            window.ShowUtility();
            return window;
        }
    }
}