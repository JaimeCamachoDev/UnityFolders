using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class UnityMaterialsCreator
{
    [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Crear Materiales", false, 23)]
    [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Crear Materiales", false, 23)]
    private static void CreateMaterialsMenu()
    {
        string folderPath = GetSelectedPathOrFallback();

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning("Por favor selecciona una carpeta válida para crear materiales.");
            return;
        }

        CreateMaterialsInFolder(folderPath);
    }

    private static void CreateMaterialsInFolder(string folderPath)
    {
        Shader shader = FindShaderByName("VZ_MAS");
        if (shader == null)
        {
            Debug.LogError("No se encontró el shader 'VZ_MAS'.");
            return;
        }

        var textureSets = new Dictionary<string, TextureSet>();
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (name.EndsWith("_MainText"))
            {
                string key = name.Substring(0, name.Length - "_MainText".Length);
                if (!textureSets.TryGetValue(key, out TextureSet set))
                    set = textureSets[key] = new TextureSet();
                set.baseMap = tex;
            }
            else if (name.EndsWith("_Normal"))
            {
                string key = name.Substring(0, name.Length - "_Normal".Length);
                if (!textureSets.TryGetValue(key, out TextureSet set))
                    set = textureSets[key] = new TextureSet();
                set.bumpMap = tex;
            }
            else if (name.EndsWith("_MAS"))
            {
                string key = name.Substring(0, name.Length - "_MAS".Length);
                if (!textureSets.TryGetValue(key, out TextureSet set))
                    set = textureSets[key] = new TextureSet();
                set.masMap = tex;
            }
        }

        foreach (KeyValuePair<string, TextureSet> kvp in textureSets)
        {
            string prefix = kvp.Key;
            TextureSet set = kvp.Value;
            string matPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, prefix + ".mat"));
            Material mat = new Material(shader);
            if (set.baseMap != null)
                mat.SetTexture("_BaseMap", set.baseMap);
            if (set.bumpMap != null)
            {
                mat.SetTexture("_BumpMap", set.bumpMap);
                mat.EnableKeyword("_NORMALMAP");
            }
            if (set.masMap != null)
                mat.SetTexture("_MAS", set.masMap);
            AssetDatabase.CreateAsset(mat, matPath);
            set.material = mat;
        }

        string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { folderPath });
        foreach (string guid in modelGuids)
        {
            string modelPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (model == null)
                continue;

            string modelPrefix = model.name;
            if (modelPrefix.EndsWith("_Geo"))
                modelPrefix = modelPrefix.Substring(0, modelPrefix.Length - 4);

            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] mats = renderer.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    string matName = mats[i] != null ? mats[i].name : modelPrefix;
                    if (textureSets.TryGetValue(matName, out TextureSet set) && set.material != null)
                    {
                        mats[i] = set.material;
                    }
                    else if (textureSets.TryGetValue(modelPrefix, out set) && set.material != null)
                    {
                        mats[i] = set.material;
                    }
                }
                renderer.sharedMaterials = mats;
                EditorUtility.SetDirty(renderer);
            }
            EditorUtility.SetDirty(model);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Materiales creados y asignados en {folderPath}.");
    }

    private static Shader FindShaderByName(string shaderName)
    {
        string[] guids = AssetDatabase.FindAssets(shaderName + " t:Shader");
        if (guids != null && guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Shader>(path);
        }
        Shader shader = Shader.Find(shaderName);
        if (shader == null)
            shader = Shader.Find("Shader Graphs/" + shaderName);
        return shader;
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

    private class TextureSet
    {
        public Texture2D baseMap;
        public Texture2D bumpMap;
        public Texture2D masMap;
        public Material material;
    }
}

