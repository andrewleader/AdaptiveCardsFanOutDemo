using FanOutUwpClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MothershipApp.Views
{
    public sealed partial class CardThumbnailView : UserControl
    {
        public CardThumbnailView()
        {
            this.InitializeComponent();

            CardCanvas.Width = App.CardWidth;
            CardContentControl.Width = App.CardWidth;
        }

        public CardViewModel CardViewModel => DataContext as CardViewModel;

        private void BorderContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double scaleRatio = e.NewSize.Width / App.CardWidth;

            CardScaleTransform.ScaleX = scaleRatio;
            CardScaleTransform.ScaleY = scaleRatio;

            UpdateContainerHeight();
        }

        private void CardBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateContainerHeight();
        }

        private void UpdateContainerHeight()
        {
            BorderContainer.Height = CardBorder.ActualHeight * CardScaleTransform.ScaleY;
        }
    }
}
