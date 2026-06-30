using UnityEngine;
using UnityEditor;
using System.IO;

public static class UnityProjectStructureCreator
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear estructura base del proyecto", false, 10)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear estructura base del proyecto", false, 10)]
    private static void CreateProjectStructure()
    {
        string root = "Assets";

        CreateFolder(root, "1-Programming");
        CreateFolder(root, "2-Art");
        CreateFolder(root, "3-Scenes");
        CreateFolder(root, "4-Presets");
        CreateFolder(root, "5-Settings");

        string artPath = Path.Combine(root, "2-Art");
        CreateFolder(artPath, "1-3D");
        CreateFolder(artPath, "2-VFX");
        CreateFolder(artPath, "3-SFX");
        CreateFolder(artPath, "4-Directors");
        CreateFolder(artPath, "5-Skybox");
        CreateFolder(artPath, "6-Videos");
        CreateFolder(artPath, "7-SolidMats");
        CreateFolder(artPath, "8-PostProcessing");
        CreateFolder(artPath, "9-UI");
        CreateFolder(artPath, "10-Lighting");

        string d3Path = Path.Combine(artPath, "1-3D");
        CreateFolder(d3Path, "Characters");
        CreateFolder(d3Path, "Environment");

        string charactersPath = Path.Combine(d3Path, "Characters");
        CreateFolder(charactersPath, "Player");
        CreateFolder(charactersPath, "NPCs");

        AssetDatabase.Refresh();
        Debug.Log("Estructura base del proyecto creada con éxito.");
    }

    private static void CreateFolder(string parentPath, string folderName)
    {
        string fullPath = Path.Combine(parentPath, folderName);
        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(parentPath, folderName);
        }
    }
}
