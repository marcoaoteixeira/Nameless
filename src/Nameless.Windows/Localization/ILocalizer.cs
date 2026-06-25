using System.ComponentModel;
using System.Globalization;

namespace Nameless.Windows.Localization;

public interface ILocalizer : INotifyPropertyChanged {
    string this[string key] { get; }

    string this[string key, params object[] parameters] { get; }

    CultureInfo CurrentCulture { get; }

    void SetCulture(CultureInfo culture);

    void SetCulture(string cultureName);
}