using app_ui_server.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;

namespace app_ui_server.Services.Hubs
{
    public class SubsribeForVehiclesWorker : BackgroundService
    {
        private readonly ConfigData _configData;
        private readonly ISubscribeService _messagesService;
        private readonly IHubContext<MotorcycleHub> _motorcyclesHub;
        private readonly IHubContext<CarHub> _carshub;

        public SubsribeForVehiclesWorker(ISubscribeService messagesService, ConfigData configData, IHubContext<MotorcycleHub> motorcyclesHub, IHubContext<CarHub> carshub)
        {
            _messagesService = messagesService;
            _configData = configData;
            _motorcyclesHub = motorcyclesHub;
            _carshub = carshub;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Use using for disposing the channels at the end
            using (var carSubscribeChannel = _messagesService.GetNewChannel())
            using (var mototrcycleSubscribeChannel = _messagesService.GetNewChannel())
            {
                // If couldn't get channels, finish the worker
                if (carSubscribeChannel == null && mototrcycleSubscribeChannel == null)
                    Console.WriteLine("Couldn't get channels to communicate with RabbitMQ server");
                // Got channels, subscribe to vehicles
                else
                {
                    if (carSubscribeChannel != null)
                        _messagesService.SubscribeForStream(
                            carSubscribeChannel,
                            _configData.ConsumeCarQueueName,
                            OnCarArrived);

                    if (mototrcycleSubscribeChannel != null)
                        _messagesService.SubscribeForStream(
                            mototrcycleSubscribeChannel,
                            _configData.ConsumeMotorcycleQueueName,
                            OnMotorcycleArrived);

                    // keep the worker alive
                    await Task.Delay(-1, stoppingToken);
                }
            }
        }

        private async void OnCarArrived(object? model, BasicDeliverEventArgs ea)
        {
            try
            {
                var carBytes = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(carBytes);
                var car = JsonConvert.DeserializeObject<Car>(message);

                // Send message to all clients who listening to the hub
                await _carshub.Clients.All.SendAsync("ReceiveMessage", message);

                Console.WriteLine($"{DateTime.Now} - Car with plate number {car?.PlateNumber} received from appX.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Got error while trying to get the car data.", e);
            }
        }

        private async void OnMotorcycleArrived(object? model, BasicDeliverEventArgs ea)
        {
            try
            {
                var motorcycleBytes = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(motorcycleBytes);
                var motorcycle = JsonConvert.DeserializeObject<Motorcycle>(message);

                // Send message to all clients who listening to the hub
                await _motorcyclesHub.Clients.All.SendAsync("ReceiveMessage", message);

                Console.WriteLine($"{DateTime.Now} - Motorcycle with plate number {motorcycle?.PlateNumber} received from appY.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Got error while trying to get the motorcycle data.", e);
            }
        }
    }
}