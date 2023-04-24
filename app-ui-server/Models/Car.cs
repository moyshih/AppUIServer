namespace app_ui_server.Models
{
    internal class Car : Vehicle
    {
        public Car(string type, string color, string plateNumber, DateTime timestamp)
            : base(type, color, plateNumber, timestamp)
        {
        }
    }
}
