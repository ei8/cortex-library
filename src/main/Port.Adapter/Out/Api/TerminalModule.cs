using ei8.Cortex.Library.Application.Notification;
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
        public TerminalModule(IEventStoreApplicationService eventStoreApplicationService) : base("/cortex/terminals")
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
        }
    }
}
