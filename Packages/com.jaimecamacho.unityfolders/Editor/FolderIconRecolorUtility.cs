using UnityEngine;
using UnityEditor;
using System.IO;

public static class FolderIconRecolorUtility
{
    public static Texture2D Recolor(Texture2D baseTex, Color tint, Texture2D overlay = null)
    {
        if (baseTex == null || !baseTex.isReadable)
        {
            Debug.LogWarning($"Texture '{baseTex?.name}' is not readable.");
            return baseTex;
        }

        Texture2D newTex = new Texture2D(baseTex.width, baseTex.height);
        Color[] basePixels = baseTex.GetPixels();

        for (int i = 0; i < basePixels.Length; i++)
        {
            Color c = basePixels[i];
            basePixels[i] = new Color(c.r * tint.r, c.g * tint.g, c.b * tint.b, c.a);
        }
        newTex.SetPixels(basePixels);

        if (overlay != null && overlay.isReadable)
        {
            int size = baseTex.width / 2;
            int startX = (baseTex.width - size) / 2;
            int startY = (baseTex.height - size) / 2;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int destX = startX + x;
                    int destY = startY + y;
                    Color overlayPixel = overlay.GetPixelBilinear(x / (float)size, y / (float)size);
                    Color basePixel = newTex.GetPixel(destX, destY);
                    newTex.SetPixel(destX, destY, Color.Lerp(basePixel, overlayPixel, overlayPixel.a));
                }
            }
        }

        newTex.Apply();
        return newTex;
    }

    public static Texture2D RecolorAndSave(Texture2D baseTex, Color tint, string name, Texture2D overlay = null)
    {
        var newTex = Recolor(baseTex, tint, overlay);
        if (newTex == null) return baseTex;

        byte[] pngData = newTex.EncodeToPNG();
        string path = $"Packages/com.jaimecamacho.unityfolders/Folders/{name}_tint.png";

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllBytes(path, pngData);

        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
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