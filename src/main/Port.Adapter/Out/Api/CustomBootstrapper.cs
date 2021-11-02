using ei8.Cortex.Graph.Client;
using ei8.Cortex.IdentityAccess.Client.Out;
using ei8.Cortex.Library.Application;
using ei8.Cortex.Library.Application.Neurons;
using ei8.Cortex.Library.Application.Notification;
using ei8.Cortex.Library.Port.Adapter.IO.Process.Services;
using ei8.EventSourcing.Client;
using Nancy;
using Nancy.TinyIoc;
using neurUL.Common.Http;

namespace ei8.Cortex.Library.Port.Adapter.Out.Api
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        public CustomBootstrapper()
        {
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<IRequestProvider, RequestProvider>();
            container.Resolve<IRequestProvider>().SetHttpClientHandler(new System.Net.Http.HttpClientHandler());

            container.Register<ISettingsService, SettingsService>();
            container.Register<INeuronGraphQueryClient, HttpNeuronGraphQueryClient>();
            container.Register<IValidationClient, HttpValidationClient>();
            container.Register<INeuronQueryService, NeuronQueryService>();
            container.Register<ITerminalQueryService, TerminalQueryService>();
            container.Register<IEventSerializer, EventSerializer>();
            container.Register<IEventStoreCoreClient, HttpEventStoreCoreClient>();
            container.Register<IEventStoreApplicationService, EventStoreApplicationService>();
        }
    }
}
