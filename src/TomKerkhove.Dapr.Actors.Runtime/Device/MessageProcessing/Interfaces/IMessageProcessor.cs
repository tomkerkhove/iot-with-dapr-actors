using System.Threading.Tasks;

namespace TomKerkhove.Dapr.Actors.Runtime.Device.MessageProcessing.Interfaces
{
    public interface IMessageProcessor
    {
        Task ProcessAsync(string rawMessage);
    }
}
