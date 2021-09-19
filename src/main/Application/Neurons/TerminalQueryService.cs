using ei8.Cortex.Graph.Client;
using ei8.Cortex.IdentityAccess.Client.Out;
using ei8.Cortex.Library.Common;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Library.Application.Neurons
{
    public class TerminalQueryService : ITerminalQueryService
    {
        private INeuronGraphQueryClient graphQueryClient;
        private IValidationClient validationClient;
        private ISettingsService settingsService;

        public TerminalQueryService(INeuronGraphQueryClient graphQueryClient, IValidationClient validationClient, ISettingsService settingsService)
        {
            AssertionConcern.AssertArgumentNotNull(graphQueryClient, nameof(graphQueryClient));
            AssertionConcern.AssertArgumentNotNull(validationClient, nameof(validationClient));
            AssertionConcern.AssertArgumentNotNull(settingsService, nameof(settingsService));

            this.graphQueryClient = graphQueryClient;
            this.validationClient = validationClient;
            this.settingsService = settingsService;
        }

        public async Task<QueryResult<Terminal>> GetTerminalById(string id, NeuronQuery neuronQuery, string userId, CancellationToken token = default)
        {
            var commonResult = await this.graphQueryClient.GetTerminalById(
               this.settingsService.CortexGraphOutBaseUrl + "/",
               id,
               neuronQuery.ToExternalType(),
               token
               );

            return await TerminalQueryService.ConvertProcessValidate(
                commonResult,
                userId,
                validationClient,
                settingsService,
                token
            );
        }

        public async Task<QueryResult<Terminal>> GetTerminals(NeuronQuery neuronQuery, string userId, CancellationToken token = default(CancellationToken))
        {
            var commonResult = await this.graphQueryClient.GetTerminals(
               this.settingsService.CortexGraphOutBaseUrl + "/",
               neuronQuery.ToExternalType(),
               token
               );

            return await TerminalQueryService.ConvertProcessValidate(
                commonResult,
                userId,
                validationClient,
                settingsService,
                token
            );
        }
        private static async Task<QueryResult<Terminal>> ConvertProcessValidate(
            Graph.Common.QueryResult commonResult, 
            string userId,
            IValidationClient validationClient, 
            ISettingsService settingsService, 
            CancellationToken token
            )
        {
            var result = commonResult.ToInternalType(n => n.Terminal.ToInternalType());
            result.Items = await result.Items.ProcessValidate(
               userId,
               validationClient,
               settingsService,
               token
               );
            return result;
        }
    }
}
