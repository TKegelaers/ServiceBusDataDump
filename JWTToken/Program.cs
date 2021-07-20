using Dalion.HttpMessageSigning.Signing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JWTToken
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = new HostBuilder()
                  .ConfigureServices((hc, s) =>
                  {
                      s.AddHttpClient();
                      s.AddTransient<GetBearerToken>();
                      s.AddTransient<SendService>();
                      s.AddHttpMessageSigning();

                  })
                  .ConfigureAppConfiguration((hc, cb) =>
                  {
                      cb.AddJsonFile("appsettings.json", optional: false);
                  });

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var getbearerToken = services.GetRequiredService<GetBearerToken>();
                    var result = await getbearerToken.Run();
                    Console.WriteLine($"BearerToken: {result}");

                    var sendService = services.GetRequiredService<SendService>();

                    //Request naar Webservice (Woningen)

                    //var requestmessage = new HttpRequestMessage();
                    //requestmessage.RequestUri = new Uri("https://iv.api.tni-vlaanderen.be/api/v1/socZek/socialeHuisvesting/socialeverhuurders/0403795657/woningen?inclusiefWoningenNietUitgevoerd=true&inclusiefWoningenUitBeheer=true&vmswWoningIds=TCQHV");
                    //requestmessage.Headers.Add("Authorization", $"Bearer {result}");
                    //requestmessage.Method = HttpMethod.Get;

                    //Message van de ServiceBus nemen                  
                    var requestmessage = new HttpRequestMessage();
                    requestmessage.RequestUri = new Uri("https://iv.api.tni-vlaanderen.be/api/v1/socZek/socialeHuisvesting/servicebus/bellk0403779975/messages/head");
                    requestmessage.Headers.Add("Authorization", $"Bearer {result}");
                    requestmessage.Method = HttpMethod.Delete;

                    var callresult = await sendService.SendAsync(requestmessage, CancellationToken.None);
                    var contentstring = await callresult.Content.ReadAsStringAsync();

                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Occured");
                }
            }
            return 0;
        }
    }
  }