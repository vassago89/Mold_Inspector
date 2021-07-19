using FirstFloor.ModernUI.Presentation;
using Mold_Inspector.Store;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFLocalizeExtension.Engine;

namespace Mold_Inspector.UI.Setting.ViewModels
{
    class CommonViewModel : BindableBase
    {
        public CommonStore CommonStore { get; }

        public LinkCollection Themes { get; }

        public IEnumerable<string> Cultures { get; }

        private string _culture;
        public string Culture
        {
            get => _culture;
            set
            {
                SetProperty(ref _culture, value);
                CommonStore.Culture = _culture;
                LocalizeDictionary.Instance.Culture = new CultureInfo(_culture);
            }
        }

        private Link _theme;
        public Link Theme
        {
            get => _theme;
            set
            {
                SetProperty(ref _theme, value);
                CommonStore.Theme = _theme.DisplayName;
                AppearanceManager.Current.ThemeSource = _theme.Source;
            }
        }

        private KeyValuePair<string, Color> _accent;
        public KeyValuePair<string, Color> Accent
        {
            get => _accent;
            set
            {
                SetProperty(ref _accent, value);
                CommonStore.AccentName = _accent.Key;
                CommonStore.AccentColor = _accent.Value;
                AppearanceManager.Current.AccentColor = _accent.Value;
            }
        }

        public IEnumerable<KeyValuePair<string, Color>> Accents =>
            typeof(Colors)
                .GetProperties()
                .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop => new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)));

        public CommonViewModel(
            CommonStore commonStore)
        {
            CommonStore = commonStore;

            Cultures = new List<string>() { "en-US", "ko-KR" };
            Culture = CommonStore.Culture;
            Themes = new LinkCollection();
            Themes.Add(new Link { DisplayName = "Dark", Source = AppearanceManager.DarkThemeSource });
            Themes.Add(new Link { DisplayName = "Light", Source = AppearanceManager.LightThemeSource });

            Theme = Themes.First(t => t.DisplayName == commonStore.Theme);
            Accent = new KeyValuePair<string, Color>(commonStore.AccentName, commonStore.AccentColor);
        }
    }
}
