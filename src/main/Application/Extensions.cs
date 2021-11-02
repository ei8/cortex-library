using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.IdentityAccess.Client.Out;
using ei8.Cortex.Library.Common;

namespace ei8.Cortex.Library.Application
{
    internal static class Extensions
    {
        internal static Graph.Common.RelativeType ToExternalType(this RelativeType value)
        {
            return (Graph.Common.RelativeType)Enum.Parse(typeof(Graph.Common.RelativeType), ((int)value).ToString());
        }

        internal static Graph.Common.NeuronQuery ToExternalType(this NeuronQuery value)
        {
            var result = new Graph.Common.NeuronQuery();
            result.Id = value.Id?.ToArray();
            result.IdNot = value.IdNot?.ToArray();
            result.Postsynaptic = value.Postsynaptic?.ToArray();
            result.PostsynapticNot = value.PostsynapticNot?.ToArray();
            result.Presynaptic = value.Presynaptic?.ToArray();
            result.PresynapticNot = value.PresynapticNot?.ToArray();
            result.TagContains = value.TagContains?.ToArray();
            result.TagContainsNot = value.TagContainsNot?.ToArray();
            result.TagContainsIgnoreWhitespace = value.TagContainsIgnoreWhitespace;
            result.RegionId = value.RegionId?.ToArray();
            result.RegionIdNot = value.RegionIdNot?.ToArray();
            result.ExternalReferenceUrl = value.ExternalReferenceUrl?.ToArray();
            result.ExternalReferenceUrlContains = value.ExternalReferenceUrlContains?.ToArray();

            result.RelativeValues = Extensions.ConvertNullableEnumToExternal<RelativeValues, Graph.Common.RelativeValues>(
                value.RelativeValues, 
                v => ((int)v).ToString()
                );

            result.NeuronActiveValues = Extensions.ConvertNullableEnumToExternal<ActiveValues, Graph.Common.ActiveValues>(
                value.NeuronActiveValues,
                v => ((int)v).ToString()
                );

            result.TerminalActiveValues = Extensions.ConvertNullableEnumToExternal<ActiveValues, Graph.Common.ActiveValues>(
                value.TerminalActiveValues,
                v => ((int)v).ToString()
                );

            result.Page = value.Page;
            result.PageSize = value.PageSize;
            
            result.SortBy = Extensions.ConvertNullableEnumToExternal<SortByValue, Graph.Common.SortByValue>(
                value.SortBy,
                v => ((int)v).ToString()
                );

            result.SortOrder = Extensions.ConvertNullableEnumToExternal<SortOrderValue, Graph.Common.SortOrderValue>(
                value.SortOrder,
                v => ((int)v).ToString()
                );

            return result;
        }

        private static TNew? ConvertNullableEnumToExternal<TOrig, TNew>(TOrig? original, Func<TOrig?, string> evaluator) 
            where TOrig : struct 
            where TNew : struct
        {
            TNew? r = null;
            if (original.HasValue)
                r = (TNew)Enum.Parse(
                    typeof(TNew),
                    evaluator(original.Value)
                    );
            return r;
        }

        internal static Neuron ToInternalType(this Graph.Common.NeuronResult value)
        {
            return value != null ? new Neuron()
            {
                Id = value.Id,
                Tag = value.Tag,
                Terminal = value.Terminal != null ? value.Terminal.ToInternalType() : null,
                Version = value.Version,
                Creation = value.Creation.ToInternalType(),
                LastModification = value.LastModification.ToInternalType(),
                UnifiedLastModification = value.UnifiedLastModification.ToInternalType(),
                Region = value.Region.ToInternalType(),
                ExternalReferenceUrl = value.ExternalReferenceUrl,
                Active = value.Active
            } :
            null;
        }

        internal static Terminal ToInternalType(this Graph.Common.Terminal value)
        {
            return new Terminal()
            {
                Effect = value.Effect,
                Id = value.Id,
                PostsynapticNeuronId = value.PostsynapticNeuronId,
                PresynapticNeuronId = value.PresynapticNeuronId,
                Strength = value.Strength,
                Creation = value.Creation.ToInternalType(),
                LastModification = value.LastModification.ToInternalType(),
                Version = value.Version,
                ExternalReferenceUrl = value.ExternalReferenceUrl,
                Active = value.Active
            };
        }

        internal static AuthorEventInfo ToInternalType(this Graph.Common.AuthorEventInfo value)
        {
            AuthorEventInfo result = null;
            if (value != null )
                result = new AuthorEventInfo()
                {
                    Author = value.Author.ToInternalType(),
                    Timestamp = value.Timestamp
                };
            
            return result;                
        }

        internal static NeuronInfo ToInternalType(this Graph.Common.NeuronInfo value)
        {
            NeuronInfo result = null;
            if (value != null)
                result = new NeuronInfo()
                {
                    Id = value.Id,
                    Tag = value.Tag
                };
            return result;
        }

        internal static Library.Common.Notification ToInternalType(this EventSourcing.Common.Notification value)
        {
            return new Library.Common.Notification()
            {
                SequenceId = value.SequenceId,
                TypeName = value.TypeName,
                Id = value.Id,                 
                Data = value.Data,
                AuthorId = value.AuthorId,
                Version = value.Version,
                Timestamp = value.Timestamp
            };
        }

        internal static QueryResult<T> ToInternalType<T>(
            this Graph.Common.QueryResult value, 
            Func<Graph.Common.NeuronResult, T> itemsSelector) where T : class
        {
            return new QueryResult<T>()
            {
                Count = value.Count,
                Items = value.Neurons.Select(n => itemsSelector(n))
            };
        }

        internal static void RestrictAccess(this Neuron value, AccessType type, string reason)
        {
            if (type == AccessType.Write)
                value.Validation.ReadOnly = true;
            else
            {
                value.Id = Guid.Empty.ToString();
                value.Tag = string.Empty;

                if (value.Terminal != null && value.Type == RelativeType.Presynaptic)
                    value.Terminal.RestrictAccess(type, reason);

                value.Active = true;
                value.Creation = Extensions.CreateAuthorEventInfo();
                value.Region = new NeuronInfo();
                value.LastModification = Extensions.CreateAuthorEventInfo();
                value.UnifiedLastModification = Extensions.CreateAuthorEventInfo();
                value.ExternalReferenceUrl = string.Empty;
                value.Version = 0;
            }

            value.Validation.RestrictionReasons = value.Validation.RestrictionReasons.Concat(new string[] { reason });
        }

        internal static void RestrictAccess(this Terminal value, AccessType type, string reason)
        {
            if (type == AccessType.Write)
            {
                value.Validation.ReadOnly = true;
            }
            else
            {                
                value.Id = Guid.Empty.ToString();
                value.PresynapticNeuronId = Guid.Empty.ToString();
                value.PostsynapticNeuronId = Guid.Empty.ToString();
                value.Effect = string.Empty;
                value.Strength = string.Empty;
                value.Version = 0;
                value.Creation = Extensions.CreateAuthorEventInfo();
                value.LastModification = Extensions.CreateAuthorEventInfo();
                value.Active = true;
                value.Url = string.Empty;
                value.ExternalReferenceUrl = string.Empty;
            }

            value.Validation.RestrictionReasons = value.Validation.RestrictionReasons.Concat(new string[] { reason });
        }

        private static AuthorEventInfo CreateAuthorEventInfo()
        {
            return new AuthorEventInfo() { Author = new NeuronInfo() };
        }

        internal async static Task<IEnumerable<Neuron>> ProcessValidate(this IEnumerable<Neuron> neurons, string userId, IValidationClient validationClient, ISettingsService settingsService, CancellationToken token)
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
                rn => {
                    rn.Validation.IsCurrentUserCreationAuthor = rn.Creation?.Author.Id == validationResults.UserNeuronId.ToString();
                    rn.Url = Uri.TryCreate(new Uri(settingsService.NeuronsUrl), rn.Id, out Uri nresult) ?
                        nresult.AbsoluteUri : 
                        throw new InvalidOperationException($"URL generation failed for Neuron with Id '{rn.Id}'");

                    if (rn.Terminal != null)
                        rn.Terminal.UpdateTerminal(validationResults.UserNeuronId.ToString(), settingsService.TerminalsUrl);
                }
                );

            return resultNeurons.ToArray();
        }

        internal async static Task<IEnumerable<Terminal>> ProcessValidate(this IEnumerable<Terminal> terminals, string userId, IValidationClient validationClient, ISettingsService settingsService, CancellationToken token)
        {
            // validate read
            var validationResults = await validationClient.ReadNeurons(
                settingsService.IdentityAccessOutBaseUrl + "/",
                terminals.Select(t => Guid.Parse(t.PresynapticNeuronId)),
                userId,
                token
                );
                        
            var resultTerminals = terminals.ToList();
            // mask neurons with errors from result set
            validationResults.NeuronValidationResults
                .Where(nv => nv.Errors.Count() > 0)
                .ToList()
                .ForEach(nv =>
                    resultTerminals.Where(te => te.PresynapticNeuronId == nv.NeuronId.ToString())
                        .ToList()
                        .ForEach(tef => tef.RestrictAccess(
                                AccessType.Read,
                                "Presynaptic Errors: " + string.Join("; ", nv.Errors.Select(e => e.Description))
                            )
                        )
                );

            resultTerminals.ToList().ForEach(
                rt => rt.UpdateTerminal(validationResults.UserNeuronId.ToString(), settingsService.TerminalsUrl)
                );

            return resultTerminals.ToArray();
        }

        private static void UpdateTerminal(this Terminal terminal, string userNeuronId, string terminalsUrl)
        {
            terminal.Validation.IsCurrentUserCreationAuthor = terminal.Creation?.Author.Id == userNeuronId;
            terminal.Url = Uri.TryCreate(new Uri(terminalsUrl), terminal.Id, out Uri tresult) ?
                tresult.AbsoluteUri : 
                throw new InvalidOperationException($"URL generation failed for Terminal with Id '{terminal.Id}'");
        }
    }
}
