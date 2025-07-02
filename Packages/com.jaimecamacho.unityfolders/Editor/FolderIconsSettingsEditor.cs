using UnityEditor;
using UnityEngine;
using System.IO;

namespace JaimeCamachoDev.UnityFolders
{
    [CustomEditor(typeof(FolderIconsSettings))]
    public class FolderIconsSettingsEditor : Editor
    {
        private FolderIconsSettings settings;

        public override void OnInspectorGUI()
        {
            settings = (FolderIconsSettings)target;

            if (settings.rules == null) return;

            EditorGUILayout.Space();

            foreach (var rule in settings.rules)
            {
                EditorGUILayout.BeginVertical("box");
                rule.match = EditorGUILayout.TextField("Match Path", rule.match);

                rule.background = EditorGUILayout.ColorField("Background & Tint", rule.background);
                rule.iconSmall = (Texture2D)EditorGUILayout.ObjectField("Icon Small", rule.iconSmall, typeof(Texture2D), false);
                rule.iconLarge = (Texture2D)EditorGUILayout.ObjectField("Icon Large", rule.iconLarge, typeof(Texture2D), false);

                EditorGUILayout.Space();

                if (GUILayout.Button("Apply Color & Generate Icons"))
                {
                    if (rule.iconSmall != null)
                        rule.iconSmall = RecolorAndSave(rule.iconSmall, rule.background, rule.match + "_Small");

                    if (rule.iconLarge != null)
                        rule.iconLarge = RecolorAndSave(rule.iconLarge, rule.background, rule.match + "_Large");

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (GUI.changed)
                EditorUtility.SetDirty(settings);
        }

        private Texture2D RecolorAndSave(Texture2D original, Color tint, string name)
        {
            if (!original.isReadable)
            {
                Debug.LogWarning($"Texture '{original.name}' is not readable.");
                return original;
            }

            Texture2D newTex = new Texture2D(original.width, original.height);
            Color[] pixels = original.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color baseColor = pixels[i];
                pixels[i] = new Color(baseColor.r * tint.r, baseColor.g * tint.g, baseColor.b * tint.b, baseColor.a);
            }

            newTex.SetPixels(pixels);
            newTex.Apply();

            byte[] pngData = newTex.EncodeToPNG();
            string path = $"Packages/com.jaimecamacho.unityfolders/Folders/{name}.png";
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllBytes(path, pngData);

            AssetDatabase.ImportAsset(path);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.GUI;
                importer.maxTextureSize = 256;
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}