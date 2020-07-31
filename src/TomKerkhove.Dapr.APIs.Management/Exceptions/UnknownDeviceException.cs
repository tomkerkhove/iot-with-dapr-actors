using System;

namespace TomKerkhove.Dapr.APIs.Management.Exceptions
{
    public class UnknownDeviceException : Exception
    {
        /// <summary>
        /// Imei of the device
        /// </summary>
        public string Imei { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imei">Imei of the device</param>
        public UnknownDeviceException(string imei)
        {
            Imei = imei;
        }
    }
}
