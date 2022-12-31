using ei8.Cortex.Library.Application.Neurons;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;

namespace ei8.Cortex.Library.Port.Adapter.Out.Api
{
    public class TerminalModule : NancyModule
    {
        public TerminalModule(ITerminalQueryService terminalQueryService) : base("/cortex/terminals")
        {
            this.Get("/{terminalid:guid}", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    var nv = await terminalQueryService.GetTerminalById(parameters.terminalid, NeuronModule.ParseNeuronQueryOrEmpty(this.Request.Url.Query), NeuronModule.GetUserId(this.Request));
                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );

            this.Get(string.Empty, async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                    {
                        var nv = await terminalQueryService.GetTerminals(NeuronModule.ParseNeuronQueryOrEmpty(this.Request.Url.Query), NeuronModule.GetUserId(this.Request));
                        return new TextResponse(JsonConvert.SerializeObject(nv));
                    }
                );
            }
            );
        }
    }
}
