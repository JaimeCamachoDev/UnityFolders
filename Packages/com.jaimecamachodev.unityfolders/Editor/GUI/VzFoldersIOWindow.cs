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
            EditorGUILayout.Space(6);
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
            BeginCard("VzFolders Configuration");

            if (PrimaryButton("Export current configuration...", height: 32))
                _mode = Mode.ExportPreview;

            EditorGUILayout.Space(4);

            if (PrimaryButton("Import configuration from file...", height: 32))
                LoadImportFile();

            EndCard();
        }

        private void DrawExportPreview()
        {
            BeginCard("Export Preview");

            var config = VzFoldersExporter.BuildConfig();
            var folderCount = config.folders.Count;
            var customIconCount = config.folders.Count(f => !string.IsNullOrEmpty(f.customIconEmbedded));

            EditorGUILayout.HelpBox(
                $"Ready to export:\n  • {folderCount} folder configurations\n  • {customIconCount} custom icons (embedded as Base64)\n  • Palette: {(config.palette != null ? "yes" : "no")}",
                MessageType.Info
            );
            EditorGUILayout.Space(8);

            if (PrimaryButton("Export to file...", height: 30))
                VzFoldersExporter.ExportToFile();

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Cancel"))
                _mode = Mode.None;

            EndCard();
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

            BeginCard("Import Preview");
            EditorGUILayout.LabelField($"Source: {_loadedConfig.exportedFrom} — {_loadedConfig.exportedAt}", EditorStyles.miniLabel);
            EndCard();

            DrawSection($"Resolved ({_importResult.Resolved.Count})", StatusGreen, () =>
            {
                foreach (var e in _importResult.Resolved)
                    EditorGUILayout.LabelField($"{e.SourcePath}  ->  {e.TargetPath}  [{e.MatchType}]", EditorStyles.miniLabel);
            });

            if (_importResult.Ambiguous.Count > 0)
            {
                DrawSection($"Ambiguous ({_importResult.Ambiguous.Count}) — choose target folder", StatusOrange, () =>
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
                DrawSection($"Not found ({_importResult.Unresolved.Count})", StatusRed, () =>
                {
                    foreach (var e in _importResult.Unresolved)
                        EditorGUILayout.LabelField($"{e.SourcePath}  —  {e.Reason}", EditorStyles.miniLabel);
                });
            }

            if (_loadedConfig.palette != null)
            {
                EditorGUILayout.Space(4);
                _importPalette = StatusToggle("Import palette colors", _importPalette);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            if (PrimaryButton("Apply Import", height: 30))
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

        private void DrawSection(string title, Color statusColor, System.Action content)
        {
            BeginCard(title, statusColor);
            content?.Invoke();
            EndCard();
        }

        // ------------------------------------------------------------------
        // Card / button styling — boxed sections with a bold header and full
        // width action buttons, matching the rest of VzFolders' look instead
        // of bare EditorGUILayout labels and default-grey buttons.
        // ------------------------------------------------------------------

        static readonly Color PrimaryBlue = new Color(0.24f, 0.42f, 0.68f);
        static readonly Color StatusGreen = new Color(0.16f, 0.5f, 0.2f);
        static readonly Color StatusOrange = new Color(0.75f, 0.45f, 0.05f);
        static readonly Color StatusRed = new Color(0.6f, 0.18f, 0.16f);
        static readonly Color StatusGrey = new Color(0.35f, 0.35f, 0.35f);

        static GUIStyle _cardStyle;
        static GUIStyle CardStyle() => _cardStyle ??= new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 8, 10) };

        static GUIStyle _primaryButtonStyle;
        static GUIStyle PrimaryButtonStyle() => _primaryButtonStyle ??= new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            hover = { textColor = Color.white },
            active = { textColor = Color.white },
        };

        void BeginCard(string title, Color? headerColor = null)
        {
            EditorGUILayout.BeginVertical(CardStyle());

            var prevColor = GUI.color;
            if (headerColor.HasValue) GUI.color = headerColor.Value;
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            GUI.color = prevColor;

            EditorGUILayout.Space(4);
        }

        void EndCard()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(6);
        }

        static bool PrimaryButton(string text, float height)
        {
            var prevBg = GUI.backgroundColor;
            GUI.backgroundColor = PrimaryBlue;
            var clicked = GUILayout.Button(text, PrimaryButtonStyle(), GUILayout.Height(height));
            GUI.backgroundColor = prevBg;
            return clicked;
        }

        // A full-width toggle rendered as a colored status bar (green "Enabled" / grey "Disabled"),
        // like the on/off chips used elsewhere for boolean options.
        static bool StatusToggle(string label, bool value)
        {
            var prevBg = GUI.backgroundColor;
            GUI.backgroundColor = value ? StatusGreen : StatusGrey;
            var clicked = GUILayout.Button($"{label} — {(value ? "Enabled" : "Disabled")}", PrimaryButtonStyle(), GUILayout.Height(24));
            GUI.backgroundColor = prevBg;
            return clicked ? !value : value;
        }
    }
}
#endif
