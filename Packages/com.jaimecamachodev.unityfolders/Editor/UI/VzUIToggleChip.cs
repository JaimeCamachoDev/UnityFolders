#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VzFolders.UI
{
    // A full-width row toggle rendered as a colored status bar: title on the left,
    // "Enabled"/"Disabled" on the right, green/red background — clicking flips it.
    // Same look as VZBoolToggle, but driven by plain get/set callbacks instead of a
    // SerializedProperty, since VzFolders' own windows aren't editing a SerializedObject.
    public class VzUIToggleChip : VisualElement
    {
        readonly Func<bool> getValue;
        readonly Action<bool> setValue;
        readonly Label stateLabel;

        public Label TitleLabel { get; }

        public VzUIToggleChip(string label, Func<bool> getValue, Action<bool> setValue, string tooltipText = null)
        {
            this.getValue = getValue;
            this.setValue = setValue;

            tooltip = tooltipText;

            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            style.justifyContent = Justify.SpaceBetween;
            style.overflow = Overflow.Hidden;

            VzUIStyle.ApplyRoundedBox(this, 8);
            VzUIStyle.ApplyPadding(this, 7, 9);
            VzUIStyle.ApplyMargin(this, 3, 3);
            VzUIStyle.ApplyScaleAndBackgroundTransition(this);
            VzUIStyle.RegisterHoverScale(this, 1.02f);

            TitleLabel = new Label(label) { tooltip = tooltip };
            TitleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            TitleLabel.style.fontSize = 11;

            stateLabel = new Label { tooltip = tooltip };
            stateLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            stateLabel.style.fontSize = 10;

            VzUIStyle.ApplyShrinkableTitleAndStateLabels(TitleLabel, stateLabel);

            Add(TitleLabel);
            Add(stateLabel);

            RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0) return;

                this.setValue(!this.getValue());

                VzUIStyle.ClickPunch(this);
                Refresh();

                evt.StopPropagation();
            });

            Refresh();
        }

        public void Refresh()
        {
            var enabled = getValue();

            stateLabel.text = enabled ? "Enabled" : "Disabled";

            style.backgroundColor = enabled ? VzUIColors.EnabledBackground : VzUIColors.DisabledBackground;
            VzUIStyle.ApplyBorderColor(this, enabled ? VzUIColors.EnabledBorder : VzUIColors.DisabledBorder);
            stateLabel.style.color = enabled ? VzUIColors.EnabledText : VzUIColors.DisabledText;
        }
    }
}
#endif
