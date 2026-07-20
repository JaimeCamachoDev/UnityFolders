#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VzFolders.UI
{
    // Shared VisualElement styling helpers (rounded borders, hover scale, click punch)
    // used by every VzFolders UI Toolkit component — ported from the team's VZEditorStyle.
    public static class VzUIStyle
    {
        public static void ApplyRoot(VisualElement element)
        {
            element.style.paddingTop = 8;
            element.style.paddingBottom = 8;
            element.style.paddingLeft = 4;
            element.style.paddingRight = 4;
        }

        public static void ApplyRoundedBox(VisualElement element, float radius = 8)
        {
            element.style.borderTopLeftRadius = radius;
            element.style.borderTopRightRadius = radius;
            element.style.borderBottomLeftRadius = radius;
            element.style.borderBottomRightRadius = radius;

            element.style.borderTopWidth = 1;
            element.style.borderBottomWidth = 1;
            element.style.borderLeftWidth = 1;
            element.style.borderRightWidth = 1;
        }

        public static void ApplyPadding(VisualElement element, int vertical = 7, int horizontal = 9)
        {
            element.style.paddingTop = vertical;
            element.style.paddingBottom = vertical;
            element.style.paddingLeft = horizontal;
            element.style.paddingRight = horizontal;
        }

        public static void ApplyMargin(VisualElement element, int top = 3, int bottom = 3)
        {
            element.style.marginTop = top;
            element.style.marginBottom = bottom;
        }

        public static void ApplyShrinkableTitleAndStateLabels(Label titleLabel, Label stateLabel, float stateLabelSpacing = 6)
        {
            titleLabel.style.flexShrink = 0;
            titleLabel.style.flexGrow = 0;

            stateLabel.style.flexShrink = 1;
            stateLabel.style.flexGrow = 0;
            stateLabel.style.minWidth = 0;
            stateLabel.style.overflow = Overflow.Hidden;
            stateLabel.style.textOverflow = TextOverflow.Ellipsis;
            stateLabel.style.whiteSpace = WhiteSpace.NoWrap;
            stateLabel.style.marginLeft = stateLabelSpacing;
        }

        public static void ApplyBorderColor(VisualElement element, Color color)
        {
            element.style.borderTopColor = color;
            element.style.borderBottomColor = color;
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;
        }

        public static void ApplyScaleAndBackgroundTransition(VisualElement element, int durationMs = 100)
        {
            element.style.transitionProperty = new StyleList<StylePropertyName>(
                new List<StylePropertyName> { new("scale"), new("background-color") }
            );

            element.style.transitionDuration = new StyleList<TimeValue>(
                new List<TimeValue> { new(durationMs, TimeUnit.Millisecond), new(durationMs, TimeUnit.Millisecond) }
            );

            element.style.transitionTimingFunction = new StyleList<EasingFunction>(
                new List<EasingFunction> { new(EasingMode.EaseOutBack), new(EasingMode.EaseOut) }
            );
        }

        public static void RegisterHoverScale(VisualElement element, float hoverScale = 1.02f)
        {
            element.RegisterCallback<MouseEnterEvent>(_ => element.style.scale = new Scale(new Vector3(hoverScale, hoverScale, 1f)));
            element.RegisterCallback<MouseLeaveEvent>(_ => element.style.scale = new Scale(Vector3.one));
        }

        public static void ClickPunch(VisualElement element, float downScale = 0.96f, float upScale = 1.02f, int delayMs = 70)
        {
            element.style.scale = new Scale(new Vector3(downScale, downScale, 1f));
            element.schedule.Execute(() => element.style.scale = new Scale(new Vector3(upScale, upScale, 1f))).ExecuteLater(delayMs);
        }
    }
}
#endif
