using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ERD.Viewer.Tools.Converters;
using ERD.Viewer.Tools.SharedControls;
using System;
using System.Diagnostics;

namespace ERD.Viewer.Tools.Specialized
{
    public class FlashableLabel : LableItem
    {
        public delegate void OnMouseClickedEvent(object sender, PointerReleasedEventArgs e);

        public event OnMouseClickedEvent? OnMouseClicked;

        private Color endBackground = ColourConverters.GetFromHex("#FFFD5E03");

        private SolidColorBrush? animationBrush;

        private IBrush? originalBrush;

        // Replaced WPF ColorAnimation with a timer-driven animation
        private DispatcherTimer? animationTimer;
        private Stopwatch? animationStopwatch;

        private long duration = 2;

        private bool isVertical = false;

        public Color EndColor
        {
            get
            {
                return endBackground;
            }

            set
            {
                endBackground = value;
            }
        }

        public long DurationSeconds
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }

        public bool IsVertical
        {
            get
            {
                return isVertical;
            }

            set
            {
                PerformRotation(value);

                isVertical = value;
            }
        }

        public void StartAnimation()
        {
            if (animationBrush != null || this.Background == null)
            {
                return;
            }

            originalBrush = Background;

            if (!(Background is SolidColorBrush solidBg))
            {
                solidBg = new SolidColorBrush(Colors.Transparent);
            }

            Color start = solidBg.Color;

            animationBrush = new SolidColorBrush(start);

            Background = animationBrush;

            animationStopwatch = Stopwatch.StartNew();

            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            animationTimer.Tick += (s, e) =>
            {
                if (animationStopwatch == null || animationBrush == null)
                    return;

                double dur = Math.Max(0.1, DurationSeconds);
                double elapsed = animationStopwatch.Elapsed.TotalSeconds;
                double cycle = elapsed % (dur * 2.0);
                double progress = cycle / dur;
                if (progress > 1.0)
                {
                    progress = 2.0 - progress; // auto-reverse
                }

                animationBrush.Color = LerpColor(start, EndColor, progress);
            };

            animationTimer.Start();
        }

        public void EndAnimation()
        {
            if (originalBrush == null)
            {
                return;
            }

            try
            {
                animationTimer?.Stop();
                animationTimer = null;

                animationStopwatch?.Stop();
                animationStopwatch = null;
            }
            catch
            {
                // ignore timer stop issues
            }

            Background = originalBrush;

            animationBrush = null;

            originalBrush = null;
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            // Only treat left-button releases like WPF's OnMouseLeftButtonUp
            var current = e.GetCurrentPoint(this);
            if (current.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                EndAnimation();

                base.OnPointerReleased(e);

                OnMouseClicked?.Invoke(this, e);
            }
            else
            {
                base.OnPointerReleased(e);
            }
        }

        private void PerformRotation(bool newValue)
        {
            if (IsVertical == newValue)
            {
                // Nothing to do
                return;
            }

            if (newValue)
            {
                // Rotate to vertical posistion
                RotateTransform rotateV = new RotateTransform(90);

                this.LayoutTransform = rotateV;

                return;
            }

            // Rotate Horizontal
            RotateTransform rotateH = new RotateTransform(0);

            this.LayoutTransform = rotateH;
        }

        private static Color LerpColor(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0.0, 1.0);
            byte A = (byte)(a.A + (b.A - a.A) * t);
            byte R = (byte)(a.R + (b.R - a.R) * t);
            byte G = (byte)(a.G + (b.G - a.G) * t);
            byte B = (byte)(a.B + (b.B - a.B) * t);
            return Color.FromArgb(A, R, G, B);
        }

        private ITransform? layoutTransform;
        public ITransform? LayoutTransform
        {
            get => layoutTransform;
            set
            {
                if (layoutTransform != value)
                {
                    layoutTransform = value;
                    this.RenderTransform = value;
                }
            }
        }
    }
}