using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Library.Application
{
    public interface ISettingsService
    {
        string CortexGraphOutBaseUrl { get; }
        string EventSourcingOutBaseUrl { get; }       
        string IdentityAccessInBaseUrl { get; }
        string IdentityAccessOutBaseUrl { get; }
    }
}
