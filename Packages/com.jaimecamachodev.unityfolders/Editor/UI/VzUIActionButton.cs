#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VzFolders.UI
{
    // A full-width rounded button with hover-scale and click-punch feedback, colored
    // blue by default (pass neutral colors for a "secondary"/Cancel-style button).
    public class VzUIActionButton : VisualElement
    {
        readonly Label labelElement;
        readonly Action onClick;

        public VzUIActionButton(
            string label,
            Action onClick,
            Color? backgroundColor = null,
            Color? borderColor = null,
            Color? textColor = null
        )
        {
            this.onClick = onClick;

            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            style.justifyContent = Justify.Center;

            VzUIStyle.ApplyRoundedBox(this, 8);
            VzUIStyle.ApplyPadding(this, 8, 9);
            VzUIStyle.ApplyMargin(this, 6, 3);
            VzUIStyle.ApplyScaleAndBackgroundTransition(this);
            VzUIStyle.RegisterHoverScale(this, 1.015f);

            SetColors(
                backgroundColor ?? VzUIColors.BlueBackground,
                borderColor ?? VzUIColors.BlueBorder,
                textColor ?? VzUIColors.BlueText
            );

            labelElement = new Label(label);
            labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;
            labelElement.style.fontSize = 11;

            Add(labelElement);

            RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0) return;

                VzUIStyle.ClickPunch(this, 0.97f, 1.015f);
                this.onClick?.Invoke();
                evt.StopPropagation();
            });
        }

        public void SetColors(Color backgroundColor, Color borderColor, Color textColor)
        {
            style.backgroundColor = backgroundColor;
            VzUIStyle.ApplyBorderColor(this, borderColor);

            if (labelElement != null)
                labelElement.style.color = textColor;
        }
    }
}
#endif
