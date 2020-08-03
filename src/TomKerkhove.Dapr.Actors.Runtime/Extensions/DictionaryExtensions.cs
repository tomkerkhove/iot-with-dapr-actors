using System.Collections.Generic;
using GuardNet;
using Microsoft.Azure.Devices.Shared;

namespace TomKerkhove.Dapr.Actors.Runtime.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     Converts a dictionary to a twin collection
        /// </summary>
        /// <param name="twinDictionary">Dictionary containing twin information</param>
        public static TwinCollection ConvertToTwinCollection(this Dictionary<string, string> twinDictionary)
        {
            Guard.NotNull(twinDictionary, nameof(twinDictionary));

            var twinCollection = new TwinCollection();

            foreach (var proper in twinDictionary)
            {
                twinCollection[proper.Key] = proper.Value;
            }

            return twinCollection;
        }
    }
}