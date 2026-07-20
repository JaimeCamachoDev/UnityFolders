#if UNITY_EDITOR
using UnityEngine;

namespace VzFolders.UI
{
    // Palette for the UI Toolkit card/button/chip components below — mirrors the
    // look used elsewhere on the team (rounded cards, blue actions, green/red status chips).
    public static class VzUIColors
    {
        public static readonly Color PanelBackground = new(0.16f, 0.16f, 0.16f, 1f);

        public static readonly Color BlueBackground = new(0.18f, 0.28f, 0.42f, 1f);
        public static readonly Color BlueBorder = new(0.45f, 0.65f, 1f, 1f);
        public static readonly Color BlueText = new(0.85f, 0.9f, 1f, 1f);

        public static readonly Color EnabledBackground = new(0.15f, 0.38f, 0.22f, 1f);
        public static readonly Color EnabledBorder = new(0.35f, 0.9f, 0.45f, 1f);
        public static readonly Color EnabledText = new(0.65f, 1f, 0.7f, 1f);

        public static readonly Color DisabledBackground = new(0.38f, 0.14f, 0.14f, 1f);
        public static readonly Color DisabledBorder = new(1f, 0.35f, 0.35f, 1f);
        public static readonly Color DisabledText = new(1f, 0.65f, 0.65f, 1f);

        public static readonly Color NeutralBackground = new(0.25f, 0.25f, 0.25f, 1f);
        public static readonly Color NeutralBorder = new(0.3f, 0.3f, 0.3f, 1f);
        public static readonly Color NeutralText = new(0.8f, 0.8f, 0.8f, 1f);

        public static readonly Color InfoText = new(0.7f, 0.7f, 0.7f, 1f);
    }
}
#endif
