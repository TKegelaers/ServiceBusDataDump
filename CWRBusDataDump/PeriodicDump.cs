using Azure.Messaging.ServiceBus;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CWRBusDataDump
{
    public  class PeriodicDump
    {
        private readonly ServiceBusSender sender;
        private readonly Microsoft.Extensions.Configuration.IConfiguration config;

        public PeriodicDump(ServiceBusSender sender, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            this.sender = sender;
            this.config = config;
        }

        public async Task Dummp()
        {
            var messageBatchSize =  Convert.ToInt32(config["messageBatchSize"]);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Starting...");
                       

            
            var intervalTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(config["periodicIntervalInSeconds"]));

            var timespan = new TimeSpan(0, Convert.ToInt32(config["periodicTimeInMinutes"]), 0);

            var i = 0;
            while(stopwatch.ElapsedTicks < timespan.Ticks )
            {
                using(var batch = await sender.CreateMessageBatchAsync())
                {
                    i++;
                    for (int j = 1; j <= messageBatchSize; j++)
                    {
                        batch.TryAddMessage(MessageHelper.CreateMessage(j));
                    }
                    var sw = new Stopwatch();
                    sw.Start();
                    Console.WriteLine($"Sending  batch {i} with {messageBatchSize} messages");

                    await sender.SendMessagesAsync(batch);

                    Console.WriteLine($"batch {i} send: {sw.Elapsed.TotalSeconds} seconds");
                    sw.Stop();
                }
                
                Console.WriteLine($"Waiting {intervalTimeSpan.Seconds} seconds...");
                await Task.Delay(intervalTimeSpan);
            }
           
            Console.WriteLine($"Total of {i * messageBatchSize} send in {stopwatch.Elapsed.TotalMinutes} minutes.");
            stopwatch.Stop();

            Console.ReadKey();
        }
    }
}
