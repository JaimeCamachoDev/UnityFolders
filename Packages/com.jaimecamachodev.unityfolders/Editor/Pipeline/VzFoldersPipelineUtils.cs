#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace VzFolders.Pipeline
{
    static class VzFoldersPipelineUtils
    {
        // The folder the current Project window selection points at, or "Assets" if nothing/no folder is selected.
        public static string GetSelectedFolderOrFallback()
        {
            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;

                return AssetDatabase.IsValidFolder(path) ? path : Path.GetDirectoryName(path).Replace("\\", "/");
            }

            return "Assets";
        }

        public static void RemoveEmptySubdirectories(string rootPath)
        {
            var directories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
                                        .OrderByDescending(d => d.Length);

            foreach (var directory in directories)
                if (Directory.GetFileSystemEntries(directory).Length == 0)
                    AssetDatabase.DeleteAsset(directory.Replace("\\", "/"));
        }
    }
}
#endif
