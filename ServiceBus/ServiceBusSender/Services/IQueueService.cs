using System.Threading.Tasks;

namespace ServiceBusSender.Services
{
    public interface IQueueService
    {
        Task SendMessageAsync<T>(T serviceBusMessage, string queueName);
    }
}