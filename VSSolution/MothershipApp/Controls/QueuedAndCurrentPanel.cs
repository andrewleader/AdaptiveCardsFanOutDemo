using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MothershipApp.Controls
{
    public class QueuedAndCurrentPanel : Panel
    {
        private const double QueuedWidth = 250;

        public QueuedAndCurrentPanel()
        {
        }

        private UIElement GetQueued()
        {
            return Children.Count < 2 ? null : Children[Children.Count - 2];
        }

        private UIElement GetCurrent()
        {
            return Children.LastOrDefault();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var queued = GetQueued();
            var current = GetCurrent();

            queued?.Measure(new Size(QueuedWidth, double.PositiveInfinity));
            current?.Measure(new Size(App.CardWidth, double.PositiveInfinity));

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var queued = GetQueued();
            var current = GetCurrent();

            queued?.Arrange(new Rect(
                x: finalSize.Width / 2 - QueuedWidth / 2, // Centered horizontally
                y: 0 - queued.DesiredSize.Height / 2, // Halfway showing from top of screen
                width: QueuedWidth,
                height: queued.DesiredSize.Height));

            current?.Arrange(new Rect(
                x: finalSize.Width / 2 - App.CardWidth / 2, // Centered horizontally
                y: finalSize.Height / 2 - current.DesiredSize.Height / 2, // Centered vertically
                width: App.CardWidth,
                height: current.DesiredSize.Height));

            return base.ArrangeOverride(finalSize);
        }
    }
}
