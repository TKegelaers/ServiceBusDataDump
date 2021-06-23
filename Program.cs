using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CWRBusDataDump
{
    class Program
    {

        static IConfiguration config;

        static async Task Main(string[] args)
        {
            config = CreateConfig();
           var sender = CreateSender();


            Console.WriteLine("Choose your dump:");
            Console.WriteLine("1: MessageDump");
            Console.WriteLine("2: PeriodicDump");

            string dump = Console.ReadLine();

            if(dump == "1")
            {
                var messagedump = new MessageDump(sender, config);
                await messagedump.Dummp();
            }

            if (dump == "2")
            {
                var periodicdump = new PeriodicDump(sender, config);
                await periodicdump.Dummp();
            }    
        }

        private static IConfiguration CreateConfig()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", false, true);
            var config = builder.Build();
            return config;
        }

        private static ServiceBusSender CreateSender()
        {
            var creds = new ClientSecretCredential(config["tenantId"], config["clientId"], config["clientSecret"]);
            var client = new ServiceBusClient(config["servicebusQualifiedName"], creds);
            var sender = client.CreateSender(config["queuename"]);

            return sender;
        }
    }



    public class Message
    {
        public string Test { get; set; }

        public Message(string test  )
        {
            Test = test;
        }
    }

}

