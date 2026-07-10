#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static VzFolders.Libs.VUtils;

namespace VzFolders.Pipeline
{
    // Prefixes every asset directly or indirectly inside a folder with that folder's name,
    // e.g. dropping Diffuse.png/Normal.png into "Rock01" renames them Rock01_Diffuse/Rock01_Normal.
    static class VzFoldersAssetsRenamer
    {
        const string dir = "Tools/JaimeCamachoDev/VzFolders/";
        const string assetsDir = "Assets/VzFolders/";

        [MenuItem(dir + "Organize/Rename assets with folder prefix", false, 222)]
        [MenuItem(assetsDir + "Rename assets with folder prefix", false, 222)]
        static void RenameAssetsMenu()
        {
            var folderPath = VzFoldersPipelineUtils.GetSelectedFolderOrFallback();
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning("VzFolders: selecciona una carpeta válida para renombrar sus assets.");
                return;
            }

            RenameAssetsInFolder(folderPath);
        }

        internal static void RenameAssetsInFolder(string folderPath)
        {
            AssetDatabase.Refresh(); // import any file dropped in via the OS file explorer before AssetDatabase can see it

            var rootName = folderPath.GetFilename(withExtension: true);
            var guids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });

            foreach (var guid in guids)
            {
                var assetPath = guid.ToPath();
                if (AssetDatabase.IsValidFolder(assetPath)) continue;

                var directory = assetPath.GetParentPath();
                var extension = assetPath.GetExtension();
                var nameNoExt = assetPath.GetFilename(withExtension: false);

                if (nameNoExt == rootName || nameNoExt.StartsWith(rootName + "_")) continue;

                var underscoreIndex = nameNoExt.IndexOf('_');
                var suffix = underscoreIndex >= 0 ? nameNoExt.Substring(underscoreIndex + 1) : nameNoExt;

                var newPath = AssetDatabase.GenerateUniqueAssetPath(directory.CombinePath(rootName + "_" + suffix + extension));
                AssetDatabase.MoveAsset(assetPath, newPath);
            }

            AssetDatabase.Refresh();
            Debug.Log($"VzFolders: assets renombrados en {folderPath}.");
        }
    }
}
#endif
