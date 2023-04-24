namespace app_ui_server.Services
{
    /// <summary>
    /// App config data with default values for case there isn't an appsettings file
    /// </summary>
    public class ConfigData
    {
        public string HostName { get; set; } = "localhost";
        public string ConsumeCarQueueName { get; set; } = "carUiQueue";
        public string ConsumeMotorcycleQueueName { get; set; } = "motorcycleUiQueue";
        public string CarHubName { get; set; } = "carHub";
        public string MotorcycleHubName { get; set; } = "motorcycleHub";
    }
}
