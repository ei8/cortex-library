using ei8.Cortex.Library.Application.Notification;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using System;

namespace ei8.Cortex.Library.Port.Adapter.Out.Api
{
    public class EventStoreModule : NancyModule
    {
        public EventStoreModule(IEventStoreApplicationService eventStoreService) : base("cortex/eventstore")
        {
            this.Get("/{aggregateid}", async (parameters) => {
                int version = 0;
                if (this.Request.Query.version.HasValue)
                    version = int.Parse(this.Request.Query.version);
                var notifs = await eventStoreService.Get(Guid.Parse(parameters.aggregateId), version);
                return new TextResponse(JsonConvert.SerializeObject(notifs));
            }
            );
        }
    }
}