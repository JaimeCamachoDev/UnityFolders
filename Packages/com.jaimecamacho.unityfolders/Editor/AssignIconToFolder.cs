using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class AssignIconToFolder
{
    [MenuItem("Assets/JaimeCamachoDev/Asignar icono a esta carpeta", true)]
    public static bool ValidateFolder()
    {
        if (Selection.activeObject == null) return false;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return AssetDatabase.IsValidFolder(path);
    }

    [MenuItem("Assets/JaimeCamachoDev/Asignar icono a esta carpeta")]
    public static void AddRuleForFolder()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string folderName = System.IO.Path.GetFileName(path);

        var settings = AssetDatabase
            .FindAssets("t:FolderIconsSettings")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(p => AssetDatabase.LoadAssetAtPath<FolderIconsSettings>(p))
            .FirstOrDefault();

        if (settings == null)
        {
            Debug.LogWarning("No se encontró un asset de configuración de FolderIconsSettings.");
            return;
        }

        Undo.RecordObject(settings, "Añadir regla de icono de carpeta");
        settings.rules.Add(new FolderIconRule
        {
            ruleName = folderName,
            match = folderName,
            matchType = MatchType.Name,
            priority = 0,
            enabled = true
        });

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();

        Selection.activeObject = settings;
        EditorGUIUtility.PingObject(settings);
    }
}
