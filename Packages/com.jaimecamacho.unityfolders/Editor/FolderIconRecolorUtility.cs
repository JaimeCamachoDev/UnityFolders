using System.IO;
using UnityEditor;
using UnityEngine;

namespace JaimeCamachoDev.UnityFolders
{
    public static class FolderIconRecolorUtility
    {
        public static Texture2D RecolorAndSave(Texture2D original, Color tint, string name)
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

            string path = $"Packages/com.jaimecamacho.unityfolders/Folders/{name}_tint.png";
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