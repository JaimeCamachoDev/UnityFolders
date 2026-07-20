#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace VzFolders.UI
{
    // A rounded card with a bold title — the outer grouping box used throughout VzFolders windows.
    public class VzUIPanel : VisualElement
    {
        public Label TitleLabel { get; }

        public VzUIPanel(string title)
        {
            VzUIStyle.ApplyRoundedBox(this, 12);
            VzUIStyle.ApplyPadding(this, 10, 10);
            VzUIStyle.ApplyMargin(this, 6, 6);

            style.backgroundColor = VzUIColors.PanelBackground;
            VzUIStyle.ApplyBorderColor(this, VzUIColors.NeutralBorder);

            TitleLabel = new Label(title);
            TitleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            TitleLabel.style.fontSize = 13;
            TitleLabel.style.marginBottom = 6;

            Add(TitleLabel);
        }
    }
}
#endif
