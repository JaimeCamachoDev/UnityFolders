#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace VzFolders.UI
{
    // Muted, wrapping helper text — used for the "ready to export"/status blurbs inside a panel.
    public class VzUIInfoLabel : Label
    {
        public VzUIInfoLabel(string text = null)
        {
            if (!string.IsNullOrEmpty(text)) this.text = text;

            style.whiteSpace = WhiteSpace.Normal;
            style.fontSize = 11;
            style.color = VzUIColors.InfoText;
            style.marginBottom = 6;
        }
    }
}
#endif
