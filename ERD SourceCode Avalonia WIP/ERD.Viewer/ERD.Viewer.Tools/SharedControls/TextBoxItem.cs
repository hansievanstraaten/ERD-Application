using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace ERD.Viewer.Tools.SharedControls
{
    public class TextBoxItem : Avalonia.Controls.TextBox
    {
        private IDisposable? foregroundSubscription;

        protected override Type StyleKeyOverride => typeof(TextBox);

        public TextBoxItem()
        {
            Avalonia.Controls.TextBox.AffectsRender<TextBoxItem>(TextProperty);
            Avalonia.Controls.TextBox.AffectsMeasure<TextBoxItem>(TextProperty);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var ns = e.NameScope;
            if (ns.Find<TextPresenter>("PART_TextPresenter") is TextPresenter tp)
            {
                tp.Foreground = this.Foreground;

                this.foregroundSubscription = this
                    .GetObservable(Avalonia.Controls.TextBox.ForegroundProperty)
                    .Subscribe(new ActionObserver<Avalonia.Media.IBrush?>(b => tp.Foreground = b));
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            this.foregroundSubscription?.Dispose();
            this.foregroundSubscription = null;
        }

        private class ActionObserver<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;
            public ActionObserver(Action<T> onNext) => _onNext = onNext;
            public void OnNext(T value) => _onNext(value);
            public void OnError(Exception error) { }
            public void OnCompleted() { }
        }
    }
}
