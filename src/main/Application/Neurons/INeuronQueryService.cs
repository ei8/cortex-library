using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Library.Common;

namespace ei8.Cortex.Library.Application.Neurons
{
    public interface INeuronQueryService
    {
        Task<IEnumerable<Neuron>> GetNeurons(NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<Neuron>> GetNeurons(string centralId, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<Neuron> GetNeuronById(string id, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<Neuron>> GetNeuronById(string id, string centralId, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken));
    }
}
