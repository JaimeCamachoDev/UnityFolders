using System.Linq;
using UnityEditor;

class FolderIconsPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedFromAssetPaths)
            .Any(p => p.EndsWith("FolderIconsSettings.asset")))
        {
            FolderIconsManager.LoadSettings();
        }
    }
}
