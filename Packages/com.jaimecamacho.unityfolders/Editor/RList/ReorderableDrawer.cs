using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityFolders.RList {

	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	public class ReorderableDrawer : PropertyDrawer {

		public const string ARRAY_PROPERTY_NAME = "array";

		private static Dictionary<int, ReorderableList> lists = new Dictionary<int, ReorderableList>();

		[Obsolete("CanCacheInspectorGUI has been deprecated and is no longer used.", false)]
		public override bool CanCacheInspectorGUI(SerializedProperty property) {

			return false;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

			ReorderableList list = GetList(property, attribute as ReorderableAttribute, ARRAY_PROPERTY_NAME);

			return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			ReorderableList list = GetList(property, attribute as ReorderableAttribute, ARRAY_PROPERTY_NAME);

			if (list != null) {

				list.DoList(EditorGUI.IndentedRect(position), label);
			}
			else {

				GUI.Label(position, "Array must extend from ReorderableArray", EditorStyles.label);
			}
		}

		public static int GetListId(SerializedProperty property) {

			if (property != null) {

				int h1 = property.serializedObject.targetObject.GetHashCode();
				int h2 = property.propertyPath.GetHashCode();

				return (((h1 << 5) + h1) ^ h2);
			}

			return 0;
		}

		public static ReorderableList GetList(SerializedProperty property, string arrayPropertyName) {

			return GetList(property, null, GetListId(property), arrayPropertyName);
		}

		public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib, string arrayPropertyName) {

			return GetList(property, attrib, GetListId(property), arrayPropertyName);
		}

		public static ReorderableList GetList(SerializedProperty property, int id, string arrayPropertyName) {

			return GetList(property, null, id, arrayPropertyName);
		}

		public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib, int id, string arrayPropertyName) {

			if (property == null) {

				return null;
			}

			ReorderableList list = null;
			SerializedProperty array = property.FindPropertyRelative(arrayPropertyName);

			if (array != null && array.isArray) {

				if (!lists.TryGetValue(id, out list)) {

					if (attrib != null) {

                                                Texture icon = !string.IsNullOrEmpty(attrib.elementIconPath)
                                                    ? AssetDatabase.GetCachedIcon(attrib.elementIconPath)
                                                    : null;

                                                list = new ReorderableList(array, attrib.add, attrib.remove, attrib.draggable);
                                                list.elementDisplayType = attrib.singleLine
                                                    ? ReorderableList.ElementDisplayType.SingleLine
                                                    : ReorderableList.ElementDisplayType.Auto;
                                                list.paginate = attrib.paginate;
                                                list.pageSize = attrib.pageSize;
                                                list.sortable = attrib.sortable;
					}
					else {

						list = new ReorderableList(array, true, true, true);
					}

					lists.Add(id, list);
				}
			}

			return list;
		}

	}
}