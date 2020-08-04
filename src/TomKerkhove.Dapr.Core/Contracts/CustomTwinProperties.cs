using System.Collections.Generic;

namespace TomKerkhove.Dapr.Core.Contracts
{
    // TODO: This is a hack and only contains some data
    // Make sure DataContractSerializer works with native Azure IoT Hub's TwinCollection & TwinProperties
    public class CustomTwinProperties
    {
        /// <summary>
        ///     Gets and sets the <see cref="T:Microsoft.Azure.Devices.Shared.Twin" /> desired properties.
        /// </summary>
        public Dictionary<string, string> Desired { get; set; } = new Dictionary<string, string>();

        /// <summary>
        ///     Gets and sets the <see cref="T:Microsoft.Azure.Devices.Shared.Twin" /> reported properties.
        /// </summary>
        public Dictionary<string, string> Reported { get; set; } = new Dictionary<string, string>();
    }
}