#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static VzFolders.Libs.VUtils;

namespace VzFolders.Pipeline
{
    // Builds URP materials from a folder of loose textures following a suffix naming convention,
    // then assigns them to the imported models sitting in the same folder.
    static class VzFoldersMaterialsCreator
    {
        const string dir = "Tools/JaimeCamachoDev/VzFolders/";
        const string assetsDir = "Assets/VzFolders/";
        const string urpLitShaderName = "Universal Render Pipeline/Lit";

        [MenuItem(dir + "Ingest pipeline/Create materials (MAS: Color, Normal, MetalSmooth, AO)", false, 240)]
        [MenuItem(assetsDir + "Ingest pipeline/Create materials (MAS)", false, 240)]
        static void CreateMasMaterialsMenu() => RunOnSelectedFolder(CreateMasMaterialsInFolder);

        [MenuItem(dir + "Ingest pipeline/Create materials (Lit: Color, Normal, Emission, MaskMap)", false, 241)]
        [MenuItem(assetsDir + "Ingest pipeline/Create materials (Lit)", false, 241)]
        static void CreateLitMaterialsMenu() => RunOnSelectedFolder(CreateLitMaterialsInFolder);

        static void RunOnSelectedFolder(System.Action<string> action)
        {
            var folderPath = VzFoldersPipelineUtils.GetSelectedFolderOrFallback();
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning("VzFolders: selecciona una carpeta válida para crear materiales.");
                return;
            }

            action(folderPath);
        }

        // "MAS" (Metallic-AO-Smoothness) style texture set: _ColorAlpha, _Normal, _MetalSmooth, _AO.
        internal static void CreateMasMaterialsInFolder(string folderPath)
        {
            var shader = FindUrpLitShader();
            if (shader == null) return;

            var suffixes = new (string suffix, System.Action<TextureSet, Texture2D> assign)[]
            {
                ("_ColorAlpha", (set, tex) => set.baseMap = tex),
                ("_Normal", (set, tex) => set.normalMap = tex),
                ("_MetalSmooth", (set, tex) => set.metalSmoothMap = tex),
                ("_AO", (set, tex) => set.occlusionMap = tex),
            };

            var textureSets = BuildTextureSets(folderPath, suffixes);
            if (textureSets.Count == 0)
            {
                Debug.LogWarning($"VzFolders: no se encontró ninguna textura en {folderPath} con los sufijos _ColorAlpha/_Normal/_MetalSmooth/_AO — no se creó ningún material.");
                return;
            }

            CreateMaterialAssets(folderPath, textureSets, shader, CreateMasMaterialAsset);
            AssignMaterialsToModels(folderPath, textureSets, shader, CreateMasMaterialAsset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"VzFolders: materiales MAS creados y asignados en {folderPath}.");
        }

        // URP Lit style texture set: _ColorAlpha, _Normal, _Emission, _MaskMap.
        internal static void CreateLitMaterialsInFolder(string folderPath)
        {
            var shader = FindUrpLitShader();
            if (shader == null) return;

            var suffixes = new (string suffix, System.Action<TextureSet, Texture2D> assign)[]
            {
                ("_ColorAlpha", (set, tex) => set.baseMap = tex),
                ("_Normal", (set, tex) => set.normalMap = tex),
                ("_Emission", (set, tex) => set.emissionMap = tex),
                ("_MaskMap", (set, tex) => set.maskMap = tex),
            };

            var textureSets = BuildTextureSets(folderPath, suffixes);
            if (textureSets.Count == 0)
            {
                Debug.LogWarning($"VzFolders: no se encontró ninguna textura en {folderPath} con los sufijos _ColorAlpha/_Normal/_Emission/_MaskMap — no se creó ningún material.");
                return;
            }

            CreateMaterialAssets(folderPath, textureSets, shader, CreateLitMaterialAsset);
            AssignMaterialsToModels(folderPath, textureSets, shader, CreateLitMaterialAsset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"VzFolders: materiales Lit creados y asignados en {folderPath}.");
        }

        static Dictionary<string, TextureSet> BuildTextureSets(string folderPath, (string suffix, System.Action<TextureSet, Texture2D> assign)[] suffixes)
        {
            AssetDatabase.Refresh(); // import any texture dropped in via the OS file explorer before AssetDatabase can see it

            var textureSets = new Dictionary<string, TextureSet>();

            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath }))
            {
                var path = guid.ToPath();
                var name = path.GetFilename(withExtension: false);
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                foreach (var (suffix, assign) in suffixes)
                {
                    if (!name.EndsWith(suffix) && name != suffix.TrimStart('_')) continue;

                    var key = name.EndsWith(suffix) ? name.Substring(0, name.Length - suffix.Length) : "";
                    if (!textureSets.TryGetValue(key, out var set))
                        set = textureSets[key] = new TextureSet();

                    assign(set, tex);
                    break;
                }
            }

            return textureSets;
        }

        static void CreateMaterialAssets(string folderPath, Dictionary<string, TextureSet> textureSets, Shader shader, System.Func<string, TextureSet, Shader, Material> createAsset)
        {
            foreach (var kvp in textureSets)
            {
                if (string.IsNullOrEmpty(kvp.Key)) continue; // the unnamed/default set is only used as a fallback for models below
                createAsset(kvp.Key.CombinePathFolder(folderPath), kvp.Value, shader);
            }
        }

        static void AssignMaterialsToModels(string folderPath, Dictionary<string, TextureSet> textureSets, Shader shader, System.Func<string, TextureSet, Shader, Material> createAsset)
        {
            textureSets.TryGetValue("", out var unnamedSet);

            foreach (var guid in AssetDatabase.FindAssets("t:Model", new[] { folderPath }))
            {
                var modelPath = guid.ToPath();
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (model == null) continue;

                var modelPrefix = model.name.EndsWith("_Geo") ? model.name.Substring(0, model.name.Length - 4) : model.name;

                foreach (var renderer in model.GetComponentsInChildren<Renderer>())
                {
                    var mats = renderer.sharedMaterials;
                    for (var i = 0; i < mats.Length; i++)
                    {
                        var matName = mats[i] != null ? mats[i].name : modelPrefix;

                        if (textureSets.TryGetValue(matName, out var set) && set.material != null)
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
                                    occlusionMap = unnamedSet.occlusionMap,
                                    emissionMap = unnamedSet.emissionMap,
                                    maskMap = unnamedSet.maskMap,
                                };
                                createAsset(modelPrefix.CombinePathFolder(folderPath), set, shader);
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
        }

        static Material CreateMasMaterialAsset(string matPath, TextureSet set, Shader shader)
        {
            if (string.IsNullOrEmpty(matPath)) return null;

            var mat = new Material(shader);
            if (set.baseMap != null) mat.SetTexture("_BaseMap", set.baseMap);
            if (set.normalMap != null) { mat.SetTexture("_BumpMap", set.normalMap); mat.EnableKeyword("_NORMALMAP"); }
            if (set.metalSmoothMap != null) { mat.SetTexture("_MetallicGlossMap", set.metalSmoothMap); mat.EnableKeyword("_METALLICSPECGLOSSMAP"); mat.SetFloat("_Metallic", 1f); }
            if (set.occlusionMap != null) mat.SetTexture("_OcclusionMap", set.occlusionMap);

            AssetDatabase.CreateAsset(mat, matPath);
            set.material = mat;
            return mat;
        }

        static Material CreateLitMaterialAsset(string matPath, TextureSet set, Shader shader)
        {
            if (string.IsNullOrEmpty(matPath)) return null;

            var mat = new Material(shader);
            if (set.baseMap != null) mat.SetTexture("_BaseMap", set.baseMap);
            if (set.normalMap != null) { mat.SetTexture("_BumpMap", set.normalMap); mat.EnableKeyword("_NORMALMAP"); }
            if (set.maskMap != null)
            {
                mat.SetTexture("_MaskMap", set.maskMap);
                mat.SetTexture("_MetallicGlossMap", set.maskMap);
                mat.SetTexture("_OcclusionMap", set.maskMap);
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

        static Shader FindUrpLitShader()
        {
            var guids = AssetDatabase.FindAssets(urpLitShaderName + " t:Shader");
            var shader = guids.Length > 0 ? AssetDatabase.LoadAssetAtPath<Shader>(guids[0].ToPath()) : null;
            shader = shader != null ? shader : Shader.Find(urpLitShaderName);

            if (shader == null) Debug.LogError($"VzFolders: no se encontró el shader '{urpLitShaderName}'.");
            return shader;
        }

        static string CombinePathFolder(this string materialName, string folderPath) =>
            AssetDatabase.GenerateUniqueAssetPath(folderPath.CombinePath(materialName + ".mat"));

        class TextureSet
        {
            public Texture2D baseMap;
            public Texture2D normalMap;
            public Texture2D metalSmoothMap;
            public Texture2D occlusionMap;
            public Texture2D emissionMap;
            public Texture2D maskMap;
            public Material material;
        }
    }
}
#endif
