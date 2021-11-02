using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ei8.Cortex.Library.Common;
using ei8.Cortex.Library.Application.Neurons;
using neurUL.Common.Domain.Model;
using ei8.Cortex.Library.Application.Notification;
using System.Web;

namespace ei8.Cortex.Library.Port.Adapter.Out.Api
{
    public class NeuronModule : NancyModule
    {
        public NeuronModule(INeuronQueryService queryService, IEventStoreApplicationService eventStoreApplicationService) : base("/cortex/neurons")
        {
            this.Get("", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {                    
                    var nv = await queryService.GetNeurons(NeuronModule.ParseNeuronQueryOrEmpty(this.Request.Url.Query), NeuronModule.GetUserId(this.Request));
                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );

            this.Get("/{neuronid:guid}", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    var nv = await queryService.GetNeuronById(parameters.neuronid, NeuronModule.ParseNeuronQueryOrEmpty(this.Request.Url.Query), NeuronModule.GetUserId(this.Request));
                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );

            this.Get("/{centralid:guid}/relatives", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    var nv = await queryService.GetNeurons(
                        parameters.centralid,
                        NeuronModule.ParseNeuronQueryOrEmpty(this.Request.Url.Query),
                        NeuronModule.GetUserId(this.Request)
                        );

                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );

            this.Get("/{centralid:guid}/relatives/{neuronid:guid}", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    var nv = await queryService.GetNeuronById(
                        parameters.neuronid,
                        parameters.centralid,
                        NeuronModule.ParseNeuronQueryOrEmpty(this.Request.Url.Query), 
                        NeuronModule.GetUserId(this.Request)
                        );
                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );

            this.Get("/{aggregateid:guid}/events", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    // TODO: validate if guid represents a neuron

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

        internal static string GetUserId(Request value)
        {
            AssertionConcern.AssertArgumentValid(k => k, (bool) value.Query["userid"].HasValue, "User Id was not found.", "userid");

            return value.Query["userid"].ToString();
        }

        internal static async Task<Response> ProcessRequest(Func<Task<Response>> action)
        {
            var result = new Response { StatusCode = HttpStatusCode.OK };

            try
            {
                result = await action();
            }
            catch (Exception ex)
            {
                result = new TextResponse(HttpStatusCode.BadRequest, ex.ToString());
            }

            return result;
        }

        internal static NeuronQuery ParseNeuronQueryOrEmpty(string queryString)
        {
            return NeuronQuery.TryParse(queryString, out NeuronQuery query) ? query : new NeuronQuery();
        }
    }
}
