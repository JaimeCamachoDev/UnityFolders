using UnityEditor;
using UnityEngine;
using System.IO;

public static class FolderIconsSettingsMenu
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear configuración")]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear configuración")]
    public static void CreateAsset()
    {
        var asset = ScriptableObject.CreateInstance<FolderIconsSettings>();
        string path = GetSelectedPathOrFallback();
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, "FolderIconsSettings.asset"));

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            string selected = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(selected)) path = Path.GetDirectoryName(selected);
            else if (Directory.Exists(selected)) path = selected;
            break;
        }
        return path;
    }
}
