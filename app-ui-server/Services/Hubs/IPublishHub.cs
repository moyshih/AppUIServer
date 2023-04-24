namespace app_ui_server.Services.Hubs
{
    public interface IPublishHub
    {
        Task SendMessage(string message);
    }
}
