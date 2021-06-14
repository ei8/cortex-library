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

namespace ei8.Cortex.Library.Port.Adapter.Out.Api
{
    public class NeuronModule : NancyModule
    {
        public NeuronModule(INeuronQueryService queryService) : base("/cortex/neurons")
        {
            this.Get("", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    var nv = await queryService.GetNeurons(NeuronModule.ExtractQuery(this.Request.Query), NeuronModule.GetUserId(this.Request));
                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );

            this.Get("/{neuronid:guid}", async (parameters) =>
            {
                return await NeuronModule.ProcessRequest(async () =>
                {
                    var nv = await queryService.GetNeuronById(parameters.neuronid, NeuronModule.ExtractQuery(this.Request.Query), NeuronModule.GetUserId(this.Request));
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
                        NeuronModule.ExtractQuery(this.Request.Query),
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
                        NeuronModule.ExtractQuery(this.Request.Query), 
                        NeuronModule.GetUserId(this.Request)
                        );
                    return new TextResponse(JsonConvert.SerializeObject(nv));
                }
                );
            }
            );
        }

        private static string GetUserId(Request value)
        {
            AssertionConcern.AssertArgumentValid(k => k, (bool) value.Query["userid"].HasValue, "User Id was not found.", "userid");

            return value.Query["userid"].ToString();
        }

        private static NeuronQuery ExtractQuery(dynamic query)
        {
            var nq = new NeuronQuery();
            nq.TagContains = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.TagContains));
            nq.TagContainsNot = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.TagContainsNot));
            nq.Id = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.Id));
            nq.IdNot = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.IdNot));
            nq.Postsynaptic = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.Postsynaptic));
            nq.PostsynapticNot = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.PostsynapticNot));
            nq.Presynaptic = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.Presynaptic));
            nq.PresynapticNot = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.PresynapticNot));
            nq.RegionId = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.RegionId));
            nq.RegionIdNot = NeuronModule.GetQueryArrayOrDefault(query, nameof(NeuronQuery.RegionIdNot));
            nq.RelativeValues = NeuronModule.GetNullableEnumValue<RelativeValues>("relative", query);
            nq.PageSize = NeuronModule.GetNullableIntValue("pagesize", query);
            nq.Page = NeuronModule.GetNullableIntValue("page", query);
            nq.NeuronActiveValues = NeuronModule.GetNullableEnumValue<ActiveValues>("nactive", query);
            nq.TerminalActiveValues = NeuronModule.GetNullableEnumValue<ActiveValues>("tactive", query);
            nq.SortBy = NeuronModule.GetNullableEnumValue<SortByValue>("sortby", query);
            nq.SortOrder = NeuronModule.GetNullableEnumValue<SortOrderValue>("sortorder", query);
            return nq;
        }

        // TODO: Transfer to common
        private static int? GetNullableIntValue(string fieldName, dynamic query)
        {
            return query[fieldName].HasValue ? int.Parse(query[fieldName].ToString()) : null;
        }

        // TODO: Transfer to common
        private static T? GetNullableEnumValue<T>(string fieldName, dynamic query) where T : struct, Enum
        {
            return query[fieldName].HasValue ? (T?)Enum.Parse(typeof(T), query[fieldName].ToString(), true) : null;
        }

        // TODO: Transfer to common, consolidate with ei8.Cortex.Library.Client.Out.HttpNeuronQueryClient helper methods
        private static IEnumerable<string> GetQueryArrayOrDefault(dynamic query, string parameterName)
        {
            var parameterNameExclamation = parameterName.Replace("Not", "!");
            string[] stringArray = query[parameterName].HasValue ?
                query[parameterName].ToString().Split(",") :
                    query[parameterNameExclamation].HasValue ?
                    query[parameterNameExclamation].ToString().Split(",") :
                    null;

            return stringArray != null ? stringArray.Select(s => s != "\0" ? s : null) : stringArray;
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
    }
}
