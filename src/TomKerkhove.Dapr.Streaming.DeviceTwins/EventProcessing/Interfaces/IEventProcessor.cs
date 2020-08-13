using System.Threading.Tasks;
using TomKerkhove.Dapr.Streaming.DeviceTwins.Contracts;

namespace TomKerkhove.Dapr.Streaming.DeviceTwins.EventProcessing.Interfaces
{
    public interface IEventProcessor
    {
        Task ProcessAsync(NotificationMetadata notificationMetadata, string rawEvent);
    }
}
