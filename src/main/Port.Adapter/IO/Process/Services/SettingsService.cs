using System;
using ei8.Cortex.Library.Application;
using ei8.Cortex.Library.Port.Adapter.Common;

namespace ei8.Cortex.Library.Port.Adapter.IO.Process.Services
{
    public class SettingsService : ISettingsService
    {
        public string CortexGraphOutBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.CortexGraphOutBaseUrl);

        public string EventSourcingOutBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.EventSourcingOutBaseUrl);

        public string IdentityAccessInBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.IdentityAccessInBaseUrl);

        public string IdentityAccessOutBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.IdentityAccessOutBaseUrl);
    }
}
