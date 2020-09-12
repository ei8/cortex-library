using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Library.Common;

namespace ei8.Cortex.Library.Application.Notification
{
    public interface IEventStoreApplicationService
    {
        Task<IEnumerable<Common.Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken token = default(CancellationToken));
    }
}
