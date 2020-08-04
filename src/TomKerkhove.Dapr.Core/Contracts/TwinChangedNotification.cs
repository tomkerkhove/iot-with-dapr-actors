using Microsoft.Azure.Devices.Shared;

namespace TomKerkhove.Dapr.Core.Contracts
{
    public class TwinChangedNotification
    {
        /// <summary>
        /// Twin's Version
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> tags.
        /// </summary>
        public TwinCollection Tags { get; set; }

        /// <summary>
        /// Gets and sets the <see cref="Twin"/> properties.
        /// </summary>
        public TwinProperties Properties { get; set; }
    }
}