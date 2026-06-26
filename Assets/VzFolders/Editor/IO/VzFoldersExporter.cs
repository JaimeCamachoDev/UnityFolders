#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace VzFolders.IO
{
    public static class VzFoldersExporter
    {
        public static void ExportToFile()
        {
            var config = BuildConfig();
            var defaultName = $"VzFolders_Config_{Application.productName}_{DateTime.Now:yyyyMMdd}.json";
            var path = EditorUtility.SaveFilePanel("Export VzFolders Configuration", "", defaultName, "json");
            if (string.IsNullOrEmpty(path)) return;

            var json = JsonUtility.ToJson(config, prettyPrint: true);
            File.WriteAllText(path, json, System.Text.Encoding.UTF8);

            Debug.Log($"[VzFolders] Configuration exported to: {path}");
            EditorUtility.RevealInFinder(path);
        }

        public static VzFoldersConfig BuildConfig()
        {
            var config = new VzFoldersConfig
            {
                exportedAt = DateTime.UtcNow.ToString("o"),
                exportedFrom = Application.productName,
                palette = BuildPaletteConfig()
            };

            var data = VzFolders.data;
            if (data == null) return config;

            config.folders = VzFoldersData.storeDataInMetaFiles
                ? BuildFolderConfigsFromMetaFiles()
                : BuildFolderConfigsFromData(data);

            return config;
        }

        static List<FolderConfig> BuildFolderConfigsFromData(VzFoldersData data)
        {
            var result = new List<FolderConfig>();
            foreach (var kvp in data.folderDatas_byGuid)
            {
                var guid = kvp.Key;
                var folderData = kvp.Value;
                var folderPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(folderPath)) continue;
                if (!AssetDatabase.IsValidFolder(folderPath)) continue;
                result.Add(BuildFolderConfig(folderPath, folderData));
            }
            return result;
        }

        static List<FolderConfig> BuildFolderConfigsFromMetaFiles()
        {
            var result = new List<FolderConfig>();
            var allFolders = AssetDatabase.FindAssets("t:Folder");
            foreach (var guid in allFolders)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path);
                if (string.IsNullOrEmpty(importer?.userData)) continue;

                VzFoldersData.FolderData folderData = null;
                try { folderData = JsonUtility.FromJson<VzFoldersData.FolderData>(importer.userData); }
                catch { continue; }

                if (folderData == null) continue;
                if (folderData.colorIndex == 0 && string.IsNullOrEmpty(folderData.iconNameOrGuid)) continue;
                result.Add(BuildFolderConfig(path, folderData));
            }
            return result;
        }

        static FolderConfig BuildFolderConfig(string folderPath, VzFoldersData.FolderData folderData)
        {
            var fc = new FolderConfig
            {
                path = folderPath,
                name = Path.GetFileName(folderPath),
                colorIndex = folderData.colorIndex,
                isColorRecursive = folderData.isColorRecursive,
                isIconRecursive = folderData.isIconRecursive,
            };
            ResolveIcon(folderData.iconNameOrGuid, fc);
            return fc;
        }

        static void ResolveIcon(string iconNameOrGuid, FolderConfig fc)
        {
            if (string.IsNullOrEmpty(iconNameOrGuid)) return;

            if (iconNameOrGuid.Length == 32 && IsHexString(iconNameOrGuid))
            {
                var iconPath = AssetDatabase.GUIDToAssetPath(iconNameOrGuid);
                if (string.IsNullOrEmpty(iconPath)) return;

                fc.customIconOriginalPath = iconPath;
                fc.customIconFileName = Path.GetFileName(iconPath);
                fc.iconName = "";

                try
                {
                    var fullPath = Path.Combine(
                        Directory.GetParent(Application.dataPath).FullName,
                        iconPath.Replace('/', Path.DirectorySeparatorChar)
                    );
                    var bytes = File.ReadAllBytes(fullPath);
                    fc.customIconEmbedded = Convert.ToBase64String(bytes);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[VzFolders] Could not embed icon '{iconPath}': {e.Message}");
                }
            }
            else
            {
                fc.iconName = iconNameOrGuid;
            }
        }

        static PaletteConfig BuildPaletteConfig()
        {
            var palette = VzFolders.palette;
            if (palette == null) return null;

            var pc = new PaletteConfig
            {
                colorSaturation = palette.colorSaturation,
                colorBrightness = palette.colorBrightness,
                colorGradientsEnabled = palette.colorGradientsEnabled,
            };

            foreach (var c in palette.colors)
                pc.colors.Add(new PaletteConfig.ColorEntry { r = c.r, g = c.g, b = c.b, a = c.a });

            return pc;
        }

        static bool IsHexString(string s)
        {
            foreach (var c in s)
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                    return false;
            return true;
        }
    }
}
#endif
