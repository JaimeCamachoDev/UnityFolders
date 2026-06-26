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

    internal static void CreateMaterialsInFolder(string folderPath)
    {
        Shader shader = FindShaderByName("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogError("No se encontró el shader 'Universal Render Pipeline/Lit'.");
            return;
        }

        var textureSets = new Dictionary<string, TextureSet>();
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (name.EndsWith("_ColorAlpha"))
            {
                string key = name.Substring(0, name.Length - "_ColorAlpha".Length);
                if (!textureSets.TryGetValue(key, out TextureSet set))
                    set = textureSets[key] = new TextureSet();
                set.baseMap = tex;
            }
            else if (name.EndsWith("_Normal"))
            {
                string key = name.Substring(0, name.Length - "_Normal".Length);
                if (!textureSets.TryGetValue(key, out TextureSet set))
                    set = textureSets[key] = new TextureSet();
                set.normalMap = tex;
            }
            else if (name.EndsWith("_MetalSmooth"))
            {
                string key = name.Substring(0, name.Length - "_MetalSmooth".Length);
                if (!textureSets.TryGetValue(key, out TextureSet set))
                    set = textureSets[key] = new TextureSet();
                set.metalSmoothMap = tex;
            }
            else if (name.EndsWith("_AO"))
            {
                string key = name.Substring(0, name.Length - "_AO".Length);
                if (!textureSets.TryGetValue(key, out TextureSet set))
                    set = textureSets[key] = new TextureSet();
                set.occlusionMap = tex;
            }
        }
        TextureSet unnamedSet = null;
        foreach (KeyValuePair<string, TextureSet> kvp in textureSets)
        {
            if (string.IsNullOrEmpty(kvp.Key))
            {
                unnamedSet = kvp.Value;
                continue;
            }

            CreateMaterialAsset(folderPath, kvp.Key, kvp.Value, shader);
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
                    else if (unnamedSet != null)
                    {
                        if (!textureSets.TryGetValue(modelPrefix, out set))
                        {
                            set = new TextureSet
                            {
                                baseMap = unnamedSet.baseMap,
                                normalMap = unnamedSet.normalMap,
                                metalSmoothMap = unnamedSet.metalSmoothMap,
                                occlusionMap = unnamedSet.occlusionMap
                            };
                            CreateMaterialAsset(folderPath, modelPrefix, set, shader);
                            textureSets[modelPrefix] = set;
                        }
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

    internal static void CreateLitMaterialsInFolder(string folderPath)
    {
        Shader shader = FindShaderByName("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogError("No se encontró el shader 'Universal Render Pipeline/Lit'.");
            return;
        }

        var textureSets = new Dictionary<string, LitTextureSet>();
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (name.EndsWith("_ColorAlpha"))
            {
                string key = name.Substring(0, name.Length - "_ColorAlpha".Length);
                if (!textureSets.TryGetValue(key, out LitTextureSet set))
                    set = textureSets[key] = new LitTextureSet();
                set.baseMap = tex;
            }
            else if (name.EndsWith("_Normal"))
            {
                string key = name.Substring(0, name.Length - "_Normal".Length);
                if (!textureSets.TryGetValue(key, out LitTextureSet set))
                    set = textureSets[key] = new LitTextureSet();
                set.normalMap = tex;
            }
            else if (name.EndsWith("_Emission"))
            {
                string key = name.Substring(0, name.Length - "_Emission".Length);
                if (!textureSets.TryGetValue(key, out LitTextureSet set))
                    set = textureSets[key] = new LitTextureSet();
                set.emissionMap = tex;
            }
            else if (name.EndsWith("_MaskMap") || name.Equals("MaskMap"))
            {
                string key = name.EndsWith("_MaskMap") ? name.Substring(0, name.Length - "_MaskMap".Length) : string.Empty;
                if (!textureSets.TryGetValue(key, out LitTextureSet set))
                    set = textureSets[key] = new LitTextureSet();
                set.maskMap = tex;
            }
        }

        LitTextureSet unnamedSet = null;
        foreach (KeyValuePair<string, LitTextureSet> kvp in textureSets)
        {
            if (string.IsNullOrEmpty(kvp.Key))
            {
                unnamedSet = kvp.Value;
                continue;
            }

            CreateLitMaterialAsset(folderPath, kvp.Key, kvp.Value, shader);
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
                    if (textureSets.TryGetValue(matName, out LitTextureSet set) && set.material != null)
                    {
                        mats[i] = set.material;
                    }
                    else if (textureSets.TryGetValue(modelPrefix, out set) && set.material != null)
                    {
                        mats[i] = set.material;
                    }
                    else if (unnamedSet != null)
                    {
                        if (!textureSets.TryGetValue(modelPrefix, out set))
                        {
                            set = new LitTextureSet
                            {
                                baseMap = unnamedSet.baseMap,
                                normalMap = unnamedSet.normalMap,
                                emissionMap = unnamedSet.emissionMap,
                                maskMap = unnamedSet.maskMap
                            };
                            CreateLitMaterialAsset(folderPath, modelPrefix, set, shader);
                            textureSets[modelPrefix] = set;
                        }
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
        Debug.Log($"Materiales LIT creados y asignados en {folderPath}.");
    }

    private static Material CreateMaterialAsset(string folderPath, string prefix, TextureSet set, Shader shader)
    {
        if (string.IsNullOrEmpty(prefix))
            return null;

        string matPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, prefix + ".mat"));
        Material mat = new Material(shader);
        if (set.baseMap != null)
            mat.SetTexture("_BaseMap", set.baseMap);
        if (set.normalMap != null)
        {
            mat.SetTexture("_BumpMap", set.normalMap);
            mat.EnableKeyword("_NORMALMAP");
        }
        if (set.metalSmoothMap != null)
        {
            mat.SetTexture("_MetallicGlossMap", set.metalSmoothMap);
            mat.EnableKeyword("_METALLICSPECGLOSSMAP");
            mat.SetFloat("_Metallic", 1f);
        }
        if (set.occlusionMap != null)
            mat.SetTexture("_OcclusionMap", set.occlusionMap);
        AssetDatabase.CreateAsset(mat, matPath);
        set.material = mat;
        return mat;
    }

    private static Material CreateLitMaterialAsset(string folderPath, string prefix, LitTextureSet set, Shader shader)
    {
        if (string.IsNullOrEmpty(prefix))
            return null;

        string matPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, prefix + ".mat"));
        Material mat = new Material(shader);
        if (set.baseMap != null)
        {
            AssignTextureIfAvailable(mat, "_BaseMap", set.baseMap);
            ValidateAssignedTexture(mat, "_BaseMap", set.baseMap, prefix, "Base Map");
        }
        if (set.normalMap != null)
        {
            AssignTextureIfAvailable(mat, "_BumpMap", set.normalMap);
            ValidateAssignedTexture(mat, "_BumpMap", set.normalMap, prefix, "Normal Map");
            mat.EnableKeyword("_NORMALMAP");
        }
        if (set.maskMap != null)
        {
            AssignTextureIfAvailable(mat, "_MaskMap", set.maskMap);
            AssignTextureIfAvailable(mat, "_MetallicGlossMap", set.maskMap);
            AssignTextureIfAvailable(mat, "_OcclusionMap", set.maskMap);
            ValidateAssignedTexture(mat, "_MaskMap", set.maskMap, prefix, "Mask Map");
            ValidateAssignedTexture(mat, "_MetallicGlossMap", set.maskMap, prefix, "Metallic Map");
            ValidateAssignedTexture(mat, "_OcclusionMap", set.maskMap, prefix, "Occlusion Map");
            mat.EnableKeyword("_METALLICSPECGLOSSMAP");
            mat.SetFloat("_Metallic", 1f);
            mat.SetFloat("_OcclusionStrength", 1f);
        }
        if (set.emissionMap != null)
        {
            mat.SetTexture("_EmissionMap", set.emissionMap);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.white);
        }
        AssetDatabase.CreateAsset(mat, matPath);
        set.material = mat;
        return mat;
    }

    private static void AssignTextureIfAvailable(Material material, string propertyName, Texture texture)
    {
        if (material.HasProperty(propertyName))
            material.SetTexture(propertyName, texture);
    }

    private static void ValidateAssignedTexture(Material material, string propertyName, Texture expectedTexture, string materialPrefix, string channelName)
    {
        if (!material.HasProperty(propertyName))
        {
            Debug.LogWarning($"El material {materialPrefix} no tiene la propiedad {propertyName} esperada para asignar la textura {channelName}.");
            return;
        }

        if (material.GetTexture(propertyName) != expectedTexture)
        {
            Debug.LogWarning($"No se pudo asignar correctamente la textura {channelName} al material {materialPrefix} en la propiedad {propertyName}.");
        }
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
        public Texture2D normalMap;
        public Texture2D metalSmoothMap;
        public Texture2D occlusionMap;
        public Material material;
    }

    private class LitTextureSet
    {
        public Texture2D baseMap;
        public Texture2D normalMap;
        public Texture2D emissionMap;
        public Texture2D maskMap;
        public Material material;
    }
}

