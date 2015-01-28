﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Ocean.Core.Configuration;
using Ocean.Core;

namespace Ocean.Framework.Themes
{
    public partial class ThemeProvider : IThemeProvider
    {
        #region Fields

        private readonly IList<ThemeConfiguration> _themeConfigurations = new List<ThemeConfiguration>();
        private string basePath = string.Empty;

        #endregion

        #region Constructors

        public ThemeProvider(OceanConfig oceanConfig, IWebHelper webHelper)
        {
            basePath = webHelper.MapPath(oceanConfig.ThemeBasePath);
            LoadConfigurations();
        }

        #endregion

        #region IThemeProvider

        public ThemeConfiguration GetThemeConfiguration(string themeName)
        {
            return
                _themeConfigurations.SingleOrDefault(
                    x => x.ThemeName.Equals(themeName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IList<ThemeConfiguration> GetThemeConfigurations()
        {
            return _themeConfigurations;
        }

        public bool ThemeConfigurationExists(string themeName)
        {
            return GetThemeConfigurations().Any(configuration => configuration.ThemeName.Equals(themeName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion

        #region Utility

        private void LoadConfigurations()
        {
            //TODO:Use IFileStorage?
            foreach (string themeName in Directory.GetDirectories(basePath))
            {
                var configuration = CreateThemeConfiguration(themeName);
                if (configuration != null)
                {
                    _themeConfigurations.Add(configuration);
                }
            }
        }

        private ThemeConfiguration CreateThemeConfiguration(string themePath)
        {
            var themeDirectory = new DirectoryInfo(themePath);
            var themeConfigFile = new FileInfo(Path.Combine(themeDirectory.FullName, "theme.config"));

            if (themeConfigFile.Exists)
            {
                var doc = new XmlDocument();
                doc.Load(themeConfigFile.FullName);
                return new ThemeConfiguration(themeDirectory.Name, themeDirectory.FullName, doc);
            }

            return null;
        }

        #endregion
    }
}