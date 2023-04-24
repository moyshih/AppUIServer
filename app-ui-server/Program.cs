using app_ui_server.Services;
using app_ui_server.Services.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add configuration to the container
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// Bind configuration values to class
var configData = new ConfigData();
configuration.GetSection("Config").Bind(configData);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(configData);
builder.Services.AddSingleton<ISubscribeService, RabbitMQSubscribeService>();
builder.Services.AddHostedService<SubsribeForVehiclesWorker>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddCors(p => p.AddPolicy(
    "corsapp", builder =>
    {
        builder.WithOrigins("https://localhost").AllowAnyMethod().AllowAnyHeader();
    }));

// Add middleware to the pipeline
var options = new DefaultFilesOptions();
options.DefaultFileNames.Clear();
options.DefaultFileNames.Add("index.html");

var app = builder.Build();

app.UseCors("corsapp");
app.UseDefaultFiles(options);
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<CarHub>(configData.CarHubName);
    endpoints.MapHub<MotorcycleHub>(configData.MotorcycleHubName);
});
app.MapControllers();

app.Run();