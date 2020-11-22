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

        public async Task<QueryResult> GetNeurons(NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeurons(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType();
            result.Neurons = await NeuronQueryService.ApplyValidation(
                subjectId, 
                result.Neurons, 
                this.validationClient, 
                this.settingsService, 
                token
                );
            return result;
        }

        private async static Task<IEnumerable<NeuronResult>> ApplyValidation(Guid subjectId, IEnumerable<NeuronResult> neurons, IValidationClient validationClient, ISettingsService settingsService, CancellationToken token)
        {
            // validate read
            var validationResults = await validationClient.ReadNeurons(
                settingsService.IdentityAccessOutBaseUrl + "/",
                neurons.Select(n => Guid.Parse(n.Id)),
                subjectId,
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

            return resultNeurons.ToArray();
        }

        public async Task<QueryResult> GetNeurons(string centralId, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeurons(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                centralId,
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType();
            result.Neurons = await NeuronQueryService.ApplyValidation(
                subjectId,
                result.Neurons,
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

        public async Task<QueryResult> GetNeuronById(string id, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeuronById(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                id,
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType();
            result.Neurons = await NeuronQueryService.ApplyValidation(
                subjectId,
                result.Neurons,
                this.validationClient,
                this.settingsService,
                token
                );
            return result;
        }

        public async Task<QueryResult> GetNeuronById(string id, string centralId, NeuronQuery neuronQuery, Guid subjectId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetNeuronById(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                id,
                centralId,
                neuronQuery.ToExternalType(),
                token
                );

            var result = commonResult.ToInternalType();
            result.Neurons = await NeuronQueryService.ApplyValidation(
                subjectId,
                result.Neurons,
                this.validationClient,
                this.settingsService,
                token
                );
            return result;
        }
    }
}
