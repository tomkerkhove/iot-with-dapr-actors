namespace TomKerkhove.Dapr.Actors.Device.Interface.Contracts
{
    public class DeviceInfo
    {
        public string IP { get; set; }
        public string IMEI { get; set; }
        public string Tenant { get; set; }
    }
}