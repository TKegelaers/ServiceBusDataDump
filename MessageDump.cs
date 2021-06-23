using Azure.Messaging.ServiceBus;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CWRBusDataDump
{
    public  class MessageDump
    {
        private readonly ServiceBusSender sender;
        private readonly Microsoft.Extensions.Configuration.IConfiguration config;

        public MessageDump(ServiceBusSender sender, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            this.sender = sender;
            this.config = config;
        }

        public async Task Dummp()
        {
            var messageBatchSize =  Convert.ToInt32(config["messageBatchSize"]);

            var batchCount = Convert.ToInt32(config["batchCount"]);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Starting...");
                       


            for (int i = 1; i <= batchCount; i++)
            {
                using (var batch = await sender.CreateMessageBatchAsync())
                {
                    for (int j = 1; j <= messageBatchSize; j++)
                    {
                        batch.TryAddMessage(MessageHelper.CreateMessage(j));
                    }
                    var sw = new Stopwatch();
                    sw.Start();
                    Console.WriteLine($"Sending  batch {i} with {messageBatchSize} messages");

                    await sender.SendMessagesAsync(batch);
                    Console.WriteLine($"batch {i} send: {sw.Elapsed.TotalSeconds}");
                    sw.Restart();
                }                    
            }
            Console.WriteLine($"Total of {batchCount * messageBatchSize} send in {stopwatch.Elapsed.TotalMinutes} minutes.");
            stopwatch.Stop();

            Console.ReadKey();
        }
    }
}
