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

            var result = commonResult.ToInternalType();
            result.Items = await NeuronQueryService.ApplyValidation(
                userId, 
                result.Items, 
                this.validationClient, 
                this.settingsService, 
                token
                );
            return result;
        }

        private async static Task<IEnumerable<Neuron>> ApplyValidation(string userId, IEnumerable<Neuron> neurons, IValidationClient validationClient, ISettingsService settingsService, CancellationToken token)
        {
            // validate read
            var validationResults = await validationClient.ReadNeurons(
                settingsService.IdentityAccessOutBaseUrl + "/",
                neurons.Select(n => Guid.Parse(n.Id)),
                userId,
                token
                );
                        
            var resultNeurons = neurons.ToList();
            // mask neurons with errors from result set
            validationResults.NeuronValidationResults
                .Where(nv => nv.Errors.Count() > 0)
                .ToList()
                .ForEach(nv =>
                    resultNeurons.Where(ne => ne.Id == nv.NeuronId.ToString())
                        .ToList()
                        .ForEach(nef => nef.RestrictAccess(
                                AccessType.Read,
                                string.Join("; ", nv.Errors.Select(e => e.Description))
                            )
                        )
                );

            resultNeurons.ToList().ForEach(
                rn => rn.Validation.IsCurrentUserCreationAuthor = rn.Creation?.Author.Id == validationResults.UserNeuronId.ToString()
                );

            return resultNeurons.ToArray();
        }

        public async Task<QueryResult<Neuron>> GetNeurons(string centralId, NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeurons(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                centralId,
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType();
            result.Items = await NeuronQueryService.ApplyValidation(
                userId,
                result.Items,
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

            var result = commonResult.ToInternalType();
            result.Items = await NeuronQueryService.ApplyValidation(
                userId,
                result.Items,
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

            var result = commonResult.ToInternalType();
            result.Items = await NeuronQueryService.ApplyValidation(
                userId,
                result.Items,
                this.validationClient,
                this.settingsService,
                token
                );
            return result;
        }
    }
}
