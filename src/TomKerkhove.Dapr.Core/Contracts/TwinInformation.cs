using System.Collections.Generic;

namespace TomKerkhove.Dapr.Core.Contracts
{
    // TODO: This is a hack and only contains some data
    // Make sure DataContractSerializer works with native Azure IoT Hub's TwinCollection & TwinProperties
    public class TwinInformation
    {
        /// <summary>
        ///     Twin's Version
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        ///     Gets and sets the <see cref="Twin" /> tags.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        /// <summary>
        ///     Gets and sets the <see cref="Twin" /> properties.
        /// </summary>
        public CustomTwinProperties Properties { get; set; } = new CustomTwinProperties();

        public static TwinInformation Parse(TwinChangedNotification twinChangedNotification)
        {
            var twinInfo = new TwinInformation
            {
                Version = twinChangedNotification.Version
            };

            if (twinChangedNotification.Tags != null)
            {
                foreach (KeyValuePair<string, object> tag in twinChangedNotification.Tags)
                {
                    twinInfo.Tags.Add(tag.Key, tag.Value.ToString());
                }
            }

            if (twinChangedNotification.Properties?.Reported != null)
            {
                foreach (KeyValuePair<string, object> reportedProperty in twinChangedNotification.Properties.Reported)
                {
                    twinInfo.Properties.Reported.Add(reportedProperty.Key, reportedProperty.Value.ToString());
                }
            }

            if (twinChangedNotification.Properties?.Desired != null)
            {
                foreach (KeyValuePair<string, object> desiredProperty in twinChangedNotification.Properties.Desired)
                {
                    twinInfo.Properties.Desired.Add(desiredProperty.Key, desiredProperty.Value.ToString());
                }
            }

            return twinInfo;
        }
    }
}