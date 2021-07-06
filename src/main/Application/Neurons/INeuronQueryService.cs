using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Library.Common;

namespace ei8.Cortex.Library.Application.Neurons
{
    public interface INeuronQueryService
    {
        Task<QueryResult<Neuron>> GetNeurons(NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken));

        Task<QueryResult<Neuron>> GetNeurons(string centralId, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken));

        Task<QueryResult<Neuron>> GetNeuronById(string id, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken));

        Task<QueryResult<Neuron>> GetNeuronById(string id, string centralId, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken));
    }
}
