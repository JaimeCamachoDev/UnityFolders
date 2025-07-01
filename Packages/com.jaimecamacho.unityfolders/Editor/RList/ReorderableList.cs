using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Borodar.RainbowFolders.RList
{
    public class ReorderableList
    {
        public enum ElementDisplayType { Auto, Expandable, SingleLine }
        public delegate void ActionDelegate(ReorderableList list);

        private readonly SerializedProperty _property;
        private readonly UnityEditorInternal.ReorderableList _list;

        public GUIContent label = new GUIContent();
        public bool paginate;
        public int pageSize = 10;
        public bool sortable;
        public bool expandable = true;
        public ElementDisplayType elementDisplayType = ElementDisplayType.Auto;

        public ReorderableList(SerializedProperty property)
            : this(property, true, true, true)
        {
        }

        public ReorderableList(SerializedProperty property, bool canAdd, bool canRemove, bool draggable)
        {
            _property = property;
            _list = new UnityEditorInternal.ReorderableList(property.serializedObject, property, draggable, true, canAdd, canRemove);

            _list.drawHeaderCallback = rect => { if (label != null) EditorGUI.LabelField(rect, label); };
            _list.drawElementCallback = (rect, index, active, focused) =>
            {
                var element = property.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };
            _list.elementHeightCallback = index => EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));
            _list.onChangedCallback += l => onChangedCallback?.Invoke(this);
        }

        public bool canAdd
        {
            get => _list.displayAdd;
            set => _list.displayAdd = value;
        }

        public bool canRemove
        {
            get => _list.displayRemove;
            set => _list.displayRemove = value;
        }

        public bool draggable
        {
            get => _list.draggable;
            set => _list.draggable = value;
        }

        public float headerHeight
        {
            get => _list.headerHeight;
            set => _list.headerHeight = value;
        }

        public float footerHeight
        {
            get => _list.footerHeight;
            set => _list.footerHeight = value;
        }

        public event ActionDelegate onChangedCallback;

        public float GetHeight() => _list.GetHeight();

        public void DoLayoutList()
        {
            // pagination is not supported in this simplified wrapper
            _list.DoLayoutList();
        }

        public void DoList(Rect rect, GUIContent label)
        {
            this.label = label;
            _list.DoList(rect);
        }
    }
}
