// Copyright 2012,2013 Vaughn Vernon
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Modifications copyright(C) 2020 ei8/Elmer Bool

using neurUL.Common.Domain.Model;
using neurUL.Common.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Library.Common;
using ei8.EventSourcing.Client;
using ei8.EventSourcing.Client.Out;
using CQRSlite.Events;
using System.Collections.Generic;
using System.Linq;

namespace ei8.Cortex.Library.Application.Notification
{
    public class EventStoreApplicationService : IEventStoreApplicationService
    {
        private readonly IEventStoreCoreClient eventStoreClient;
        private readonly ISettingsService settingsService;

        public EventStoreApplicationService(IEventStoreCoreClient eventStoreClient, ISettingsService settingsService)
        {
            AssertionConcern.AssertArgumentNotNull(eventStoreClient, nameof(eventStoreClient));
            this.eventStoreClient = eventStoreClient;
            this.settingsService = settingsService;
        }

        public async Task<IEnumerable<Common.Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken token = default)
        {
            return (await this.eventStoreClient.Get(
                    this.settingsService.EventSourcingOutBaseUrl + "/",
                    aggregateId,
                    fromVersion,
                    token
                    )
                ).Select(n => n.ToInternalType());
        }
    }
}
