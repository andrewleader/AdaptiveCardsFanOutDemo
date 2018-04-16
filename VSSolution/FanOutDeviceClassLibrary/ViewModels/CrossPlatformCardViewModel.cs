using FanOutClassLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace FanOutDeviceClassLibrary.ViewModels
{
    public abstract class CrossPlatformCardViewModel : BindableBase, IEquatable<CrossPlatformCardViewModel>
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public Guid CardIdentifier { get; set; } = Guid.NewGuid();

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

        protected abstract void Render();

        public CrossPlatformCardViewModel Clone()
        {
            var model = CreateInstanceFunction();
            model.Name = Name;
            model.CardJson = CardJson;
            return model;
        }

        public bool Equals(CrossPlatformCardViewModel other)
        {
            return CardJson == other.CardJson;
        }

        public static Func<CrossPlatformCardViewModel> CreateInstanceFunction;
    }
}
