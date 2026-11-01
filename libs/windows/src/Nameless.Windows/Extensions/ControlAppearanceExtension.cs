using Nameless.Windows.Extensions.Resources;
using Wpf.Ui.Controls;

namespace Nameless.Windows;

/// <summary>
///     <see cref="ControlAppearance"/> extensions methods.
/// </summary>
public static class ControlAppearanceExtension {
    /// <param name="self">
    ///     The current <see cref="ControlAppearance"/>.
    /// </param>
    extension(ControlAppearance self) {
        /// <summary>
        ///     Retrieves the text title associated with the
        ///     specified <see cref="ControlAppearance"/>.
        /// </summary>
        /// <returns>
        ///     The text title associated with the <see cref="ControlAppearance"/>.
        /// </returns>
        public string Title => self switch {
            ControlAppearance.Success => ControlAppearanceResources.Title_Success,
            ControlAppearance.Caution => ControlAppearanceResources.Title_Caution,
            ControlAppearance.Danger => ControlAppearanceResources.Title_Danger,
            _ => ControlAppearanceResources.Title_Info
        };

        /// <summary>
        ///     Retrieves the icon associated with the
        ///     specified <see cref="ControlAppearance"/>.
        /// </summary>
        /// <returns>
        ///     The icon associated with the <see cref="ControlAppearance"/>.
        /// </returns>
        public IconElement Icon => self switch {
            ControlAppearance.Success => new SymbolIcon(SymbolRegular.ThumbLike28),
            ControlAppearance.Caution => new SymbolIcon(SymbolRegular.Warning28),
            ControlAppearance.Danger => new SymbolIcon(SymbolRegular.ShieldError24),
            _ => new SymbolIcon(SymbolRegular.LightbulbFilament24),
        };
    }
}