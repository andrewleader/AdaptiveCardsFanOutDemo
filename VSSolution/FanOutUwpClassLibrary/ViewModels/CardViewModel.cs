using AdaptiveCards.Rendering.Uwp;
using FanOutClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FanOutUwpClassLibrary.ViewModels
{
    public class CardViewModel : BindableBase, IEquatable<CardViewModel>
    {
        public static string HOST_CONFIG_JSON;

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _cardJson;
        public string CardJson
        {
            get { return _cardJson; }
            set
            {
                if (SetProperty(ref _cardJson, value))
                {
                    Render();
                }
            }
        }

        private FrameworkElement _cardFrameworkElement;
        public FrameworkElement CardFrameworkElement
        {
            get { return _cardFrameworkElement; }
            private set { SetProperty(ref _cardFrameworkElement, value); }
        }

        private void Render()
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

        public CardViewModel Clone()
        {
            return new CardViewModel()
            {
                Name = Name,
                CardJson = CardJson
            };
        }

        public bool Equals(CardViewModel other)
        {
            return CardJson == other.CardJson;
        }
    }
}
