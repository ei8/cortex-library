using ei8.Cortex.Library.Common;
using ei8.Cortex.Graph.Client;
using ei8.Cortex.IdentityAccess.Client.Out;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Library.Application.Neurons
{
    public class NeuronQueryService : INeuronQueryService
    {
        private INeuronGraphQueryClient graphQueryClient;
        private IValidationClient validationClient;
        private ISettingsService settingsService;

        public NeuronQueryService(INeuronGraphQueryClient graphQueryClient, IValidationClient validationClient, ISettingsService settingsService)
        {
            AssertionConcern.AssertArgumentNotNull(graphQueryClient, nameof(graphQueryClient));
            AssertionConcern.AssertArgumentNotNull(validationClient, nameof(validationClient));
            AssertionConcern.AssertArgumentNotNull(settingsService, nameof(settingsService));

            this.graphQueryClient = graphQueryClient;
            this.validationClient = validationClient;
            this.settingsService = settingsService;            
        }

        public async Task<QueryResult<Neuron>> GetNeurons(NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeurons(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType(n => n.ToInternalType());
            result.Items = await result.Items.ProcessValidate(
                userId, 
                this.validationClient, 
                this.settingsService, 
                token
                );
            return result;
        }
        
        public async Task<QueryResult<Neuron>> GetNeurons(string centralId, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeurons(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                centralId,
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType(n => n.ToInternalType());
            result.Items = await result.Items.ProcessValidate(
                userId,
                this.validationClient,
                this.settingsService,
                token
                );
            return result;
        }

        private static Guid? GetNullableStringGuid(string value)
        {
            return (value == null ? (Guid?) null : Guid.Parse(value));
        }

        public async Task<QueryResult<Neuron>> GetNeuronById(string id, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeuronById(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                id,
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType(n => n.ToInternalType());
            result.Items = await result.Items.ProcessValidate(
                userId,
                this.validationClient,
                this.settingsService,
                token
                );
            return result;
        }

        public async Task<QueryResult<Neuron>> GetNeuronById(string id, string centralId, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeuronById(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                id,
                centralId,
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType(n => n.ToInternalType());
            result.Items = await result.Items.ProcessValidate(
                userId,
                this.validationClient,
                this.settingsService,
                token
                );
            return result;
        }
    }
}
