using ei8.Cortex.Library.Application.Neurons;
using ei8.Cortex.Library.Application.Notification;
using ei8.Cortex.Library.Common;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Library.Port.Adapter.Out.Api
{
    public class TerminalModule : NancyModule
    {
        public TerminalModule(ITerminalQueryService terminalQueryService, IEventStoreApplicationService eventStoreApplicationService) : base("/cortex/terminals")
        {
            this.Get("/{aggregateid:guid}/events", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    // TODO: validate if guid represents a terminal

                    var nv = await eventStoreApplicationService.Get(
                        parameters.aggregateid,
                        0
                        );

                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );

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
