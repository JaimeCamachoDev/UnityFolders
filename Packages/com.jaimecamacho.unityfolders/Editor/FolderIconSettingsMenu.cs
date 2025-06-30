using UnityEngine;
using UnityEditor;
using System.IO;

public static class FolderIconSettingsMenu
{
    [MenuItem("Tools/JaimeCamachoDev/Crear FolderIconSettings")]
    [MenuItem("Assets/JaimeCamachoDev/Crear FolderIconSettings")]
    public static void CreateSettingsAsset()
    {
        var asset = ScriptableObject.CreateInstance<FolderIconSettings>();

        string path = GetSelectedPathOrFallback();
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, "FolderIconSettings.asset"));

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private static string GetSelectedPathOrFallback()
    {
        var path = "Assets";
        foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                path = Path.GetDirectoryName(path);
            break;
        }
        return path;
    }
}
