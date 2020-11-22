using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Library.Common;

namespace ei8.Cortex.Library.Application.Neurons
{
    public interface INeuronQueryService
    {
        Task<QueryResult> GetNeurons(NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<QueryResult> GetNeurons(string centralId, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<QueryResult> GetNeuronById(string id, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<QueryResult> GetNeuronById(string id, string centralId, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));
    }
}
