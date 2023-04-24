using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace app_ui_server.Services
{
    public interface ISubscribeService
    {
        bool IsConnected { get; }
        IModel? GetNewChannel();
        void SubscribeForStream(IModel channel, string streamName, EventHandler<BasicDeliverEventArgs> OnMessageArrived);
    }
}