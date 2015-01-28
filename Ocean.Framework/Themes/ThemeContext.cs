using System.Linq;
using Ocean.Core;

namespace Ocean.Framework.Themes
{
    //<summary>
    //Theme context
    //</summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IThemeProvider _themeProvider;

        private bool _desktopThemeIsCached;
        private string _cachedDesktopThemeName;

        private bool _mobileThemeIsCached;
        private string _cachedMobileThemeName;

        public ThemeContext(IThemeProvider themeProvider)
        {
            this._themeProvider = themeProvider;
        }

        /// <summary>
        /// Get or set current theme for desktops (e.g. darkOrange)
        /// </summary>
        public string WorkingDesktopTheme
        {
            get
            {
                if (_desktopThemeIsCached)
                    return _cachedDesktopThemeName;

                string theme = "";

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = "Default";

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                    theme = _themeProvider.GetThemeConfigurations()
                        .Where(x => !x.MobileTheme)
                        .FirstOrDefault()
                        .ThemeName;

                //cache theme
                this._cachedDesktopThemeName = theme;
                this._desktopThemeIsCached = true;
                return theme;
            }
            set
            {
                return;
                //clear cache
                this._desktopThemeIsCached = false;
            }
        }

        /// <summary>
        /// Get current theme for mobile (e.g. Mobile)
        /// </summary>
        public string WorkingMobileTheme
        {
            get
            {
                if (_mobileThemeIsCached)
                    return _cachedMobileThemeName;

                //default store theme
                string theme = "Mobile";

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                    theme = _themeProvider.GetThemeConfigurations()
                        .Where(x => x.MobileTheme)
                        .FirstOrDefault()
                        .ThemeName;

                //cache theme
                this._cachedMobileThemeName = theme;
                this._mobileThemeIsCached = true;
                return theme;
            }
        }
    }
}