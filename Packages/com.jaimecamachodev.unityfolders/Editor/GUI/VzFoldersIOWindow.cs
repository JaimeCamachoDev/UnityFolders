#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VzFolders.IO
{
    public class VzFoldersIOWindow : EditorWindow
    {
        private enum Mode { None, ExportPreview, ImportPreview }
        private Mode _mode = Mode.None;

        private VzFoldersImporter.ImportResult _importResult;
        private string _loadedConfigPath;
        private VzFoldersConfig _loadedConfig;

        private bool _importPalette = true;
        private Vector2 _scroll;

        private Dictionary<VzFoldersImporter.AmbiguousEntry, string> _ambiguousResolutions = new();

        public static void OpenForExport()
        {
            var w = GetWindow<VzFoldersIOWindow>("VzFolders Export");
            w.minSize = new Vector2(480, 300);
            w._mode = Mode.ExportPreview;
            w.Show();
        }

        public static void OpenForImport()
        {
            var w = GetWindow<VzFoldersIOWindow>("VzFolders Import");
            w.minSize = new Vector2(480, 400);
            w._mode = Mode.None;
            w.Show();
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            switch (_mode)
            {
                case Mode.None: DrawInitialButtons(); break;
                case Mode.ExportPreview: DrawExportPreview(); break;
                case Mode.ImportPreview: DrawImportPreview(); break;
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawInitialButtons()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("VzFolders Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);

            if (GUILayout.Button("Export current configuration...", GUILayout.Height(32)))
                _mode = Mode.ExportPreview;

            EditorGUILayout.Space(4);

            if (GUILayout.Button("Import configuration from file...", GUILayout.Height(32)))
                LoadImportFile();
        }

        private void DrawExportPreview()
        {
            EditorGUILayout.LabelField("Export Preview", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            var config = VzFoldersExporter.BuildConfig();
            var folderCount = config.folders.Count;
            var customIconCount = config.folders.Count(f => !string.IsNullOrEmpty(f.customIconEmbedded));

            EditorGUILayout.HelpBox(
                $"Ready to export:\n  • {folderCount} folder configurations\n  • {customIconCount} custom icons (embedded as Base64)\n  • Palette: {(config.palette != null ? "yes" : "no")}",
                MessageType.Info
            );
            EditorGUILayout.Space(8);

            if (GUILayout.Button("Export to file...", GUILayout.Height(30)))
                VzFoldersExporter.ExportToFile();

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Cancel"))
                _mode = Mode.None;
        }

        private void LoadImportFile()
        {
            var path = EditorUtility.OpenFilePanel("Import VzFolders Configuration", "", "json");
            if (string.IsNullOrEmpty(path)) return;

            _loadedConfig = VzFoldersImporter.LoadConfig(path);
            if (_loadedConfig == null) return;

            _loadedConfigPath = path;
            _importResult = VzFoldersImporter.Resolve_Internal(_loadedConfig);
            _ambiguousResolutions.Clear();
            _mode = Mode.ImportPreview;
        }

        private void DrawImportPreview()
        {
            if (_importResult == null) { _mode = Mode.None; return; }

            EditorGUILayout.LabelField("Import Preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Source: {_loadedConfig.exportedFrom} — {_loadedConfig.exportedAt}", EditorStyles.miniLabel);
            EditorGUILayout.Space(6);

            DrawSection($"Resolved ({_importResult.Resolved.Count})", Color.green, () =>
            {
                foreach (var e in _importResult.Resolved)
                    EditorGUILayout.LabelField($"{e.SourcePath}  ->  {e.TargetPath}  [{e.MatchType}]", EditorStyles.miniLabel);
            });

            if (_importResult.Ambiguous.Count > 0)
            {
                DrawSection($"Ambiguous ({_importResult.Ambiguous.Count}) — choose target folder", new Color(1f, 0.6f, 0f), () =>
                {
                    foreach (var e in _importResult.Ambiguous)
                    {
                        EditorGUILayout.LabelField($"Source: {e.SourcePath}", EditorStyles.boldLabel);
                        if (!_ambiguousResolutions.TryGetValue(e, out var chosen)) chosen = "(skip)";
                        var options = new List<string> { "(skip)" };
                        options.AddRange(e.Candidates);
                        var idx = options.IndexOf(chosen);
                        if (idx < 0) idx = 0;
                        var newIdx = EditorGUILayout.Popup("Target folder", idx, options.ToArray());
                        _ambiguousResolutions[e] = options[newIdx];
                        EditorGUILayout.Space(4);
                    }
                });
            }

            if (_importResult.Unresolved.Count > 0)
            {
                DrawSection($"Not found ({_importResult.Unresolved.Count})", Color.red, () =>
                {
                    foreach (var e in _importResult.Unresolved)
                        EditorGUILayout.LabelField($"{e.SourcePath}  —  {e.Reason}", EditorStyles.miniLabel);
                });
            }

            if (_loadedConfig.palette != null)
            {
                EditorGUILayout.Space(4);
                _importPalette = EditorGUILayout.Toggle("Import palette colors", _importPalette);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply Import", GUILayout.Height(30)))
                ApplyImport();

            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                _mode = Mode.None;
                _importResult = null;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ApplyImport()
        {
            VzFoldersImporter.Apply(_importResult, _importPalette);
            foreach (var kvp in _ambiguousResolutions)
            {
                if (kvp.Value == "(skip)") continue;
                VzFoldersImporter.ApplyAmbiguous(kvp.Key, kvp.Value);
            }
            _mode = Mode.None;
            _importResult = null;
            Close();
        }

        private void DrawSection(string title, Color titleColor, System.Action content)
        {
            EditorGUILayout.Space(4);
            var prevColor = GUI.color;
            GUI.color = titleColor;
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            GUI.color = prevColor;
            EditorGUI.indentLevel++;
            content?.Invoke();
            EditorGUI.indentLevel--;
        }
    }
}
#endif
