#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using VzFolders.UI;

namespace VzFolders.IO
{
    public class VzFoldersIOWindow : EditorWindow
    {
        private enum Mode { None, ExportPreview, ImportPreview }
        private Mode _mode = Mode.None;

        private VzFoldersImporter.ImportResult _importResult;
        private VzFoldersConfig _loadedConfig;

        private bool _importPalette = true;

        private Dictionary<VzFoldersImporter.AmbiguousEntry, string> _ambiguousResolutions = new();

        private ScrollView _root;

        public static void OpenForExport()
        {
            var w = GetWindow<VzFoldersIOWindow>("VzFolders Export");
            w.minSize = new Vector2(480, 300);
            w.SetMode(Mode.ExportPreview);
            w.Show();
        }

        public static void OpenForImport()
        {
            var w = GetWindow<VzFoldersIOWindow>("VzFolders Import");
            w.minSize = new Vector2(480, 400);
            w.SetMode(Mode.None);
            w.Show();
        }

        private void CreateGUI()
        {
            _root = new ScrollView();
            VzUIStyle.ApplyRoot(_root);
            rootVisualElement.Add(_root);

            Rebuild();
        }

        // Reused window instances (Unity may hand back the same GetWindow<T> instance) need an
        // explicit Rebuild when switching modes, since CreateGUI only runs once per window lifetime.
        private void SetMode(Mode mode)
        {
            _mode = mode;
            if (_root != null) Rebuild();
        }

        private void Rebuild()
        {
            _root.Clear();
            switch (_mode)
            {
                case Mode.None: DrawInitialButtons(); break;
                case Mode.ExportPreview: DrawExportPreview(); break;
                case Mode.ImportPreview: DrawImportPreview(); break;
            }
        }

        private void DrawInitialButtons()
        {
            var panel = new VzUIPanel("VzFolders Configuration");

            panel.Add(new VzUIActionButton("Export current configuration...", () => SetMode(Mode.ExportPreview)));
            panel.Add(new VzUIActionButton("Import configuration from file...", LoadImportFile));

            _root.Add(panel);
        }

        private void DrawExportPreview()
        {
            var panel = new VzUIPanel("Export Preview");

            var config = VzFoldersExporter.BuildConfig();
            var folderCount = config.folders.Count;
            var customIconCount = config.folders.Count(f => !string.IsNullOrEmpty(f.customIconEmbedded));

            panel.Add(new VzUIInfoLabel(
                $"Ready to export:\n  • {folderCount} folder configurations\n  • {customIconCount} custom icons (embedded as Base64)\n  • Palette: {(config.palette != null ? "yes" : "no")}"
            ));

            panel.Add(new VzUIActionButton("Export to file...", () => VzFoldersExporter.ExportToFile()));
            panel.Add(SecondaryButton("Cancel", () => SetMode(Mode.None)));

            _root.Add(panel);
        }

        private void LoadImportFile()
        {
            var path = EditorUtility.OpenFilePanel("Import VzFolders Configuration", "", "json");
            if (string.IsNullOrEmpty(path)) return;

            var config = VzFoldersImporter.LoadConfig(path);
            if (config == null) return;

            _loadedConfig = config;
            _importResult = VzFoldersImporter.Resolve_Internal(_loadedConfig);
            _ambiguousResolutions.Clear();
            SetMode(Mode.ImportPreview);
        }

        private void DrawImportPreview()
        {
            if (_importResult == null) { SetMode(Mode.None); return; }

            var summary = new VzUIPanel("Import Preview");
            summary.Add(new VzUIInfoLabel($"Source: {_loadedConfig.exportedFrom} — {_loadedConfig.exportedAt}"));
            _root.Add(summary);

            DrawSection($"Resolved ({_importResult.Resolved.Count})", VzUIColors.EnabledBorder, () =>
                _importResult.Resolved.Select(e => (VisualElement)new VzUIInfoLabel($"{e.SourcePath}  ->  {e.TargetPath}  [{e.MatchType}]")));

            if (_importResult.Ambiguous.Count > 0)
            {
                DrawSection($"Ambiguous ({_importResult.Ambiguous.Count}) — choose target folder", new Color(1f, 0.6f, 0f), () =>
                    _importResult.Ambiguous.Select(e => (VisualElement)BuildAmbiguousRow(e)));
            }

            if (_importResult.Unresolved.Count > 0)
            {
                DrawSection($"Not found ({_importResult.Unresolved.Count})", VzUIColors.DisabledBorder, () =>
                    _importResult.Unresolved.Select(e => (VisualElement)new VzUIInfoLabel($"{e.SourcePath}  —  {e.Reason}")));
            }

            if (_loadedConfig.palette != null)
            {
                var paletteToggle = new VzUIToggleChip(
                    "Import palette colors",
                    () => _importPalette,
                    value => _importPalette = value
                );
                _root.Add(paletteToggle);
            }

            var actionsRow = new VisualElement();
            actionsRow.style.flexDirection = FlexDirection.Row;
            actionsRow.style.marginTop = 6;

            var applyButton = new VzUIActionButton("Apply Import", ApplyImport);
            applyButton.style.flexGrow = 1;
            actionsRow.Add(applyButton);

            actionsRow.Add(SecondaryButton("Cancel", () =>
            {
                _importResult = null;
                SetMode(Mode.None);
            }));

            _root.Add(actionsRow);
        }

        private VisualElement BuildAmbiguousRow(VzFoldersImporter.AmbiguousEntry e)
        {
            var row = new VisualElement();

            row.Add(new VzUIInfoLabel($"Source: {e.SourcePath}"));

            if (!_ambiguousResolutions.TryGetValue(e, out var chosen)) chosen = "(skip)";
            var options = new List<string> { "(skip)" };
            options.AddRange(e.Candidates);
            if (!options.Contains(chosen)) chosen = "(skip)";

            var dropdown = new DropdownField("Target folder", options, options.IndexOf(chosen));
            dropdown.RegisterValueChangedCallback(evt => _ambiguousResolutions[e] = evt.newValue);
            _ambiguousResolutions[e] = chosen;

            row.Add(dropdown);
            return row;
        }

        private void ApplyImport()
        {
            VzFoldersImporter.Apply(_importResult, _importPalette);
            foreach (var kvp in _ambiguousResolutions)
            {
                if (kvp.Value == "(skip)") continue;
                VzFoldersImporter.ApplyAmbiguous(kvp.Key, kvp.Value);
            }
            _importResult = null;
            Close();
        }

        // A card whose header is tinted with statusColor and whose body is built by contentBuilder —
        // used for the Resolved (green) / Ambiguous (orange) / Not found (red) breakdowns.
        private void DrawSection(string title, Color statusColor, System.Func<IEnumerable<VisualElement>> contentBuilder)
        {
            var panel = new VzUIPanel(title);
            panel.TitleLabel.style.color = statusColor;

            foreach (var element in contentBuilder())
                panel.Add(element);

            _root.Add(panel);
        }

        private static VzUIActionButton SecondaryButton(string label, System.Action onClick) =>
            new(label, onClick, VzUIColors.NeutralBackground, VzUIColors.NeutralBorder, VzUIColors.NeutralText);
    }
}
#endif
