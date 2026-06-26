#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace VzFolders.IO
{
    public static class VzFoldersImporter
    {
        public class ImportResult
        {
            public List<ResolvedEntry> Resolved = new();
            public List<UnresolvedEntry> Unresolved = new();
            public List<AmbiguousEntry> Ambiguous = new();
            public bool PaletteImported;
        }

        public class ResolvedEntry
        {
            public string SourcePath;
            public string TargetPath;
            public MatchType MatchType;
            public FolderConfig Config;
        }

        public class UnresolvedEntry
        {
            public string SourcePath;
            public FolderConfig Config;
            public string Reason;
        }

        public class AmbiguousEntry
        {
            public string SourcePath;
            public List<string> Candidates;
            public FolderConfig Config;
        }

        public enum MatchType
        {
            ExactPath,
            ExactName,
            PartialPath,
        }

        public static ImportResult Preview(string jsonPath)
        {
            var config = LoadConfig(jsonPath);
            return config == null ? null : Resolve_Internal(config);
        }

        public static void Apply(ImportResult result, bool importPalette = true)
        {
            if (result == null) return;

            var data = VzFolders.data;
            if (data == null)
            {
                Debug.LogError("[VzFolders] No VzFoldersData asset found. Create one first.");
                return;
            }

            Undo.RecordObject(data, "VzFolders Import Configuration");

            int applied = 0;
            foreach (var entry in result.Resolved)
            {
                ApplyFolderConfig(entry.TargetPath, entry.Config, data);
                applied++;
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            VzFolders.folderInfoCache.Clear();
            EditorApplication.RepaintProjectWindow();

            Debug.Log($"[VzFolders] Import complete. Applied {applied} folder configs. " +
                      $"Unresolved: {result.Unresolved.Count}. Ambiguous: {result.Ambiguous.Count}.");
        }

        public static void ApplyAmbiguous(AmbiguousEntry entry, string chosenPath)
        {
            var data = VzFolders.data;
            if (data == null) return;

            Undo.RecordObject(data, "VzFolders Import Folder Config");
            ApplyFolderConfig(chosenPath, entry.Config, data);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            VzFolders.folderInfoCache.Clear();
            EditorApplication.RepaintProjectWindow();
        }

        public static VzFoldersConfig LoadConfig(string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"[VzFolders] Config file not found: {jsonPath}");
                return null;
            }

            try
            {
                var json = File.ReadAllText(jsonPath, System.Text.Encoding.UTF8);
                var config = JsonUtility.FromJson<VzFoldersConfig>(json);

                if (config?.vzFoldersConfig == null)
                {
                    Debug.LogError("[VzFolders] Invalid config file format.");
                    return null;
                }

                return config;
            }
            catch (Exception e)
            {
                Debug.LogError($"[VzFolders] Failed to parse config: {e.Message}");
                return null;
            }
        }

        public static ImportResult Resolve_Internal(VzFoldersConfig config)
        {
            var result = new ImportResult();
            var allFolders = BuildFolderIndex();

            foreach (var fc in config.folders)
            {
                var resolved = TryResolve(fc, allFolders);

                if (resolved.exactPath != null)
                    result.Resolved.Add(new ResolvedEntry { SourcePath = fc.path, TargetPath = resolved.exactPath, MatchType = MatchType.ExactPath, Config = fc });
                else if (resolved.uniqueName != null)
                    result.Resolved.Add(new ResolvedEntry { SourcePath = fc.path, TargetPath = resolved.uniqueName, MatchType = MatchType.ExactName, Config = fc });
                else if (resolved.partialPath != null)
                    result.Resolved.Add(new ResolvedEntry { SourcePath = fc.path, TargetPath = resolved.partialPath, MatchType = MatchType.PartialPath, Config = fc });
                else if (resolved.candidates.Count > 1)
                    result.Ambiguous.Add(new AmbiguousEntry { SourcePath = fc.path, Candidates = resolved.candidates, Config = fc });
                else
                    result.Unresolved.Add(new UnresolvedEntry { SourcePath = fc.path, Config = fc, Reason = "No matching folder found in this project" });
            }

            if (config.palette != null)
                result.PaletteImported = ApplyPaletteConfig(config.palette);

            return result;
        }

        struct ResolveCandidate
        {
            public string exactPath;
            public string uniqueName;
            public string partialPath;
            public List<string> candidates;
        }

        static ResolveCandidate TryResolve(FolderConfig fc, Dictionary<string, List<string>> folderIndex)
        {
            var r = new ResolveCandidate { candidates = new List<string>() };

            if (AssetDatabase.IsValidFolder(fc.path))
            {
                r.exactPath = fc.path;
                return r;
            }

            var folderName = Path.GetFileName(fc.path);
            if (folderIndex.TryGetValue(folderName, out var matchesByName))
            {
                if (matchesByName.Count == 1)
                {
                    r.uniqueName = matchesByName[0];
                    return r;
                }

                var sourceParts = fc.path.Split('/');
                string bestPartial = null;
                int bestScore = 0;

                foreach (var candidate in matchesByName)
                {
                    var candidateParts = candidate.Split('/');
                    var score = CountMatchingSuffixSegments(sourceParts, candidateParts);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPartial = candidate;
                    }
                }

                if (bestScore >= 2 && bestPartial != null)
                {
                    r.partialPath = bestPartial;
                    return r;
                }

                r.candidates = matchesByName;
                return r;
            }

            return r;
        }

        static int CountMatchingSuffixSegments(string[] source, string[] target)
        {
            int count = 0;
            int si = source.Length - 1;
            int ti = target.Length - 1;
            while (si >= 0 && ti >= 0)
            {
                if (!string.Equals(source[si], target[ti], StringComparison.OrdinalIgnoreCase)) break;
                count++; si--; ti--;
            }
            return count;
        }

        static Dictionary<string, List<string>> BuildFolderIndex()
        {
            var index = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var guids = AssetDatabase.FindAssets("t:Folder");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileName(path);
                if (!index.TryGetValue(name, out var list))
                    index[name] = list = new List<string>();
                list.Add(path);
            }
            return index;
        }

        static void ApplyFolderConfig(string targetPath, FolderConfig fc, VzFoldersData data)
        {
            var guid = AssetDatabase.AssetPathToGUID(targetPath);
            if (string.IsNullOrEmpty(guid)) return;

            if (!data.folderDatas_byGuid.TryGetValue(guid, out var folderData))
                folderData = data.folderDatas_byGuid[guid] = new VzFoldersData.FolderData();

            folderData.colorIndex = fc.colorIndex;
            folderData.isColorRecursive = fc.isColorRecursive;
            folderData.isIconRecursive = fc.isIconRecursive;

            if (!string.IsNullOrEmpty(fc.iconName))
                folderData.iconNameOrGuid = fc.iconName;
            else if (!string.IsNullOrEmpty(fc.customIconEmbedded))
                folderData.iconNameOrGuid = ImportEmbeddedIcon(fc) ?? "";
            else
                folderData.iconNameOrGuid = "";
        }

        static string ImportEmbeddedIcon(FolderConfig fc)
        {
            try
            {
                var bytes = Convert.FromBase64String(fc.customIconEmbedded);
                var importDir = "Assets/VzFolders/ImportedIcons";
                if (!AssetDatabase.IsValidFolder("Assets/VzFolders"))
                    AssetDatabase.CreateFolder("Assets", "VzFolders");
                if (!AssetDatabase.IsValidFolder(importDir))
                    AssetDatabase.CreateFolder("Assets/VzFolders", "ImportedIcons");

                var fileName = string.IsNullOrEmpty(fc.customIconFileName) ? "imported_icon.png" : fc.customIconFileName;
                var assetPath = $"{importDir}/{fileName}";
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                var fullPath = Path.Combine(
                    Directory.GetParent(Application.dataPath).FullName,
                    assetPath.Replace('/', Path.DirectorySeparatorChar)
                );

                File.WriteAllBytes(fullPath, bytes);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                return AssetDatabase.AssetPathToGUID(assetPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[VzFolders] Could not import embedded icon '{fc.customIconFileName}': {e.Message}");
                return null;
            }
        }

        static bool ApplyPaletteConfig(PaletteConfig pc)
        {
            var palette = VzFolders.palette;
            if (palette == null) return false;

            Undo.RecordObject(palette, "VzFolders Import Palette");
            palette.colors.Clear();
            foreach (var ce in pc.colors)
                palette.colors.Add(new UnityEngine.Color(ce.r, ce.g, ce.b, ce.a));

            palette.colorSaturation = pc.colorSaturation;
            palette.colorBrightness = pc.colorBrightness;
            palette.colorGradientsEnabled = pc.colorGradientsEnabled;

            EditorUtility.SetDirty(palette);
            AssetDatabase.SaveAssets();
            return true;
        }
    }
}
#endif
