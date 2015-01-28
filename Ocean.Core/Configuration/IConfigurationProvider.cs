using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Core.Configuration
{
    public interface IConfigurationProvider<TSettings> where TSettings : ISettings, new()
    {
        TSettings Settings { get; }
        void SaveSettings(TSettings settings);
        void DeleteSettings();
    }
}