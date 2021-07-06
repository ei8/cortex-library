﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            result.RegionId = value.RegionId?.ToArray();
            result.RegionIdNot = value.RegionIdNot?.ToArray();

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
                Active = value.Active
            } :
            null;
        }

        private static Terminal ToInternalType(this Graph.Common.Terminal value)
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

        internal static QueryResult<Terminal> ToQueryResult(this Graph.Common.Terminal value)
        {
            return new QueryResult<Terminal>()
            {
                Count = 1,
                Items = new Terminal[] { value.ToInternalType() }
            };
        }

        internal static QueryResult<Neuron> ToInternalType(this Graph.Common.QueryResult value)
        {
            return new QueryResult<Neuron>()
            {
                Count = value.Count,
                Items = value.Neurons.Select(n => n.ToInternalType())
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

                if (value.Terminal != null)
                    value.Terminal = new Terminal() { Creation = Extensions.CreateAuthorEventInfo() };

                value.Active = true;
                value.Creation = Extensions.CreateAuthorEventInfo();
                value.Region = new NeuronInfo();
                value.LastModification = Extensions.CreateAuthorEventInfo();
                value.UnifiedLastModification = Extensions.CreateAuthorEventInfo();
                value.Version = 0;
            }

            value.Validation.RestrictionReasons = value.Validation.RestrictionReasons.Concat(new string[] { reason });
        }

        private static AuthorEventInfo CreateAuthorEventInfo()
        {
            return new AuthorEventInfo() { Author = new NeuronInfo() };
        }
    }
}
