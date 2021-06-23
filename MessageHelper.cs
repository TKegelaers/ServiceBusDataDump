using Azure.Messaging.ServiceBus;

namespace CWRBusDataDump
{
    public static  class MessageHelper
    {
        public static ServiceBusMessage CreateMessage(int counter)
        {
            var message = new Message($"Hello World {counter}");
            var messageasstring = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            return new ServiceBusMessage(messageasstring) { ContentType = "application/json" };
        }
    }
}
