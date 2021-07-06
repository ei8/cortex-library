using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Library.Application.Neurons
{
    public interface ITerminalQueryService
    {
        Task<QueryResult<Terminal>> GetTerminalById(string id, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken));
    }
}
