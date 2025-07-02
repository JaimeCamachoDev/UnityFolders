using UnityEditor;
using UnityEngine;
using System.IO;

namespace JaimeCamachoDev.UnityFolders
{
    public class FolderIconRecolorUtility : EditorWindow
    {
        private Texture2D originalTexture;
        private Texture2D previewTexture;
        private Color tint = Color.white;
        private string saveName = "NewIcon";
        private string savePath = "Packages/com.jaimecamacho.unityfolders/Folders/";

        [MenuItem("Tools/JaimeCamachoDev/UnityFolders/Generate Colored Icon")]
        [MenuItem("Assets/JaimeCamachoDev/UnityFolders/Generate Colored Icon")]
        private static void OpenWindow()
        {
            GetWindow<FolderIconRecolorUtility>("Generate Icon");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Texture Colour Replacement", EditorStyles.boldLabel);
            originalTexture = (Texture2D)EditorGUILayout.ObjectField("Original Texture", originalTexture, typeof(Texture2D), false);

            if (originalTexture != null && !originalTexture.isReadable)
            {
                EditorGUILayout.HelpBox("⚠️ This texture is not readable. Enable 'Read/Write' in the Import Settings.", MessageType.Warning);
                return;
            }

            tint = EditorGUILayout.ColorField("Replacement Colour", tint);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            if (previewTexture != null)
            {
                GUILayout.Label(previewTexture, GUILayout.Width(64), GUILayout.Height(64));
            }

            if (GUILayout.Button("Generate"))
            {
                previewTexture = GenerateTintedTexture(originalTexture, tint);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save Created Texture", EditorStyles.boldLabel);
            saveName = EditorGUILayout.TextField("Texture Name", saveName);
            savePath = EditorGUILayout.TextField("Save Path", savePath);

            if (GUILayout.Button("Save Texture") && previewTexture != null)
            {
                string fullPath = Path.Combine(savePath, saveName + ".png");
                SaveTextureAsPNG(previewTexture, fullPath);
                AssetDatabase.Refresh();
                SetTextureImporterSettings(fullPath);
            }
        }

        private Texture2D GenerateTintedTexture(Texture2D source, Color tint)
        {
            Texture2D newTex = new Texture2D(source.width, source.height);
            Color[] pixels = source.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color original = pixels[i];
                pixels[i] = new Color(original.r * tint.r, original.g * tint.g, original.b * tint.b, original.a);
            }

            newTex.SetPixels(pixels);
            newTex.Apply();
            return newTex;
        }

        private void SaveTextureAsPNG(Texture2D texture, string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(fullPath, pngData);
        }

        private void SetTextureImporterSettings(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.GUI;
                importer.maxTextureSize = 256;
                importer.isReadable = true;
                importer.SaveAndReimport();
            }
        }
    }
}