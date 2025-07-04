using UnityEngine;
using UnityEditor;
using System.IO;

public static class UnityFoldersCreator
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear carpetas", false, 10)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear carpetas", false, 10)]
    private static void CreateUnityFolders()
    {
        string folderPath = GetSelectedPathOrFallback();

        // Abrir ventana de input directamente
        string mainFolderName = FolderDialogUtility.DisplayDialogInput("Nombre de la Carpeta", "Introduce el nombre de la carpeta principal:", "New Folder");
        if (string.IsNullOrEmpty(mainFolderName))
        {
            Debug.Log("Creación de carpetas cancelada.");
            return;
        }

        string uniqueFolderPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, mainFolderName));
        AssetDatabase.CreateFolder(folderPath, Path.GetFileName(uniqueFolderPath));
        AssetDatabase.Refresh();

        CreateSubfolders(uniqueFolderPath);
        Debug.Log($"Carpeta creada en: {uniqueFolderPath}");
    }

    private static void CreateSubfolders(string parentFolderPath)
    {
        string[] subfolders = new string[]
        {
            "Animation",
            "Audio",
            "Material",
            "Mesh",
            "Prefab",
            "Script",
            "Shader",
            "VFX"
        };

        foreach (string subfolder in subfolders)
        {
            AssetDatabase.CreateFolder(parentFolderPath, subfolder);
        }

        AssetDatabase.Refresh();
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}

public static class FolderDialogUtility
{
    public static string DisplayDialogInput(string title, string message, string defaultText)
    {
        return InputDialog.Show(title, message, defaultText);
    }
}

public class InputDialog : EditorWindow
{
    private static string input = "";
    private static string result = null;
    private static string message;
    private static string title;
    private static string defaultText;

    public static string Show(string title, string message, string defaultText)
    {
        InputDialog.title = title;
        InputDialog.message = message;
        InputDialog.defaultText = defaultText;
        input = defaultText;

        var window = ScriptableObject.CreateInstance<InputDialog>();
        window.titleContent = new GUIContent(title);
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 120);
        window.ShowModal();
        return result;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(message, EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        input = EditorGUILayout.TextField("Nombre:", input);
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Aceptar"))
        {
            result = input;
            Close();
        }
        if (GUILayout.Button("Cancelar"))
        {
            result = null;
            Close();
        }
        GUILayout.EndHorizontal();
    }
}
