﻿using System;
using System.Configuration;

using NuClear.Settings.API;

namespace NuClear.AdvancedSearch.Settings
{
    [Obsolete]
    public interface IConnectionStringSettings : ISettings
    {
        string GetConnectionString(ConnectionStringName connectionStringNameAlias);
        ConnectionStringSettings GetConnectionStringSettings(ConnectionStringName connectionStringNameAlias);
    }
}
