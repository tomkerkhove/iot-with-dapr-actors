using System.Threading.Tasks;
using TomKerkhove.Dapr.DeviceTwin.Monitor.Contracts;

namespace TomKerkhove.Dapr.DeviceTwin.Monitor.EventProcessing.Interfaces
{
    public interface IEventProcessor
    {
        Task ProcessAsync(NotificationMetadata notificationMetadata, string rawEvent);
    }
}
