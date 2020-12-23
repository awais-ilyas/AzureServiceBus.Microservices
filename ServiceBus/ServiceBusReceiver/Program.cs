using Microsoft.Azure.ServiceBus;
using ServiceBusShared.Models;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusReceiver
{
    class Program
    {
        const string connectionString = "Endpoint=sb://{servicebusname}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={SharedAccessKey}";
        const string queueName = "personqueue";
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            queueClient = new QueueClient(connectionString, queueName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false // Will not mark the message as complete, wait until wait the message and then says Yes mark as read!
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.ReadLine();

            await queueClient.CloseAsync();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken arg2)
        {
            var jsonString = Encoding.UTF8.GetString(message.Body);
            PersonModel person = JsonSerializer.Deserialize<PersonModel>(jsonString);

            // here is the place to send an email and other notifications or update in database.
            Console.WriteLine($"Person Received: {person.FirstName} {person.LastName}");

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler exception: {arg.Exception}");
            return Task.CompletedTask;
        }
    }
}
