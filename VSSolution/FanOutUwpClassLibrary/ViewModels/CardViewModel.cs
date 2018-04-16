using AdaptiveCards.Rendering.Uwp;
using FanOutClassLibrary;
using FanOutDeviceClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FanOutUwpClassLibrary.ViewModels
{
    public class CardViewModel : CrossPlatformCardViewModel
    {
        public static string HOST_CONFIG_JSON;

        private FrameworkElement _cardFrameworkElement;
        public FrameworkElement CardFrameworkElement
        {
            get { return _cardFrameworkElement; }
            private set { SetProperty(ref _cardFrameworkElement, value); }
        }

        protected override void Render()
        {
            if (CardJson == null)
            {
                CardFrameworkElement = null;
                return;
            }

            try
            {
                var renderer = new AdaptiveCardRenderer();

                if (HOST_CONFIG_JSON != null)
                {
                    var hostParseResult = AdaptiveHostConfig.FromJsonString(HOST_CONFIG_JSON);
                    if (hostParseResult.HostConfig != null)
                    {
                        renderer.HostConfig = hostParseResult.HostConfig;
                    }
                }

                var renderedCard = renderer.RenderAdaptiveCardFromJsonString(CardJson);
                CardFrameworkElement = renderedCard.FrameworkElement;
            }
            catch { }
        }
    }
}
