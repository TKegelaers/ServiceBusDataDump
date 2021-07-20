using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JWTToken
{
    public class GetBearerToken
    {
        private HttpClient httpClient { get; set; }
        private readonly string endpoint = "https://beta.oauth.vlaanderen.be/authorization/ws/oauth/v2/token";
        private readonly IConfiguration configuration;

        public GetBearerToken(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.httpClient.BaseAddress = new Uri(endpoint);

        }

        public async Task<string> Run()
        {
            var privatekey = "omg.vlaanderen.be_vmsw_socialehuisvesting-socialeverhuurders-aip.p12";
            var clientId = "6594";
            var privatex509 = new X509Certificate2(File.ReadAllBytes(privatekey), configuration["privateKeyPass"]);

            var signedToken = JsonWebTokenHandler.CreateJwtClientAssertion(privatex509, clientId, endpoint);

            var data = new Dictionary<string, string>
            {
                { "grant_type","client_credentials"},
                {"scope" ,"soczek_socHuisv_v1_G soczek_socHuisv_v1_D"},
                {"client_assertion_type","urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                {"client_assertion",signedToken }
            };

            var content = new FormUrlEncodedContent(data);

            var respone = await httpClient.PostAsync("", content).ConfigureAwait(false);
            var contentstring = await respone.Content.ReadAsStringAsync();
            var result = JObject.Parse(contentstring).ToObject<BearerTokenResult>();

            return result.Access_token;
        }


        public class BearerTokenResult
        {
            public string Access_token { get; set; }
            public string Scope { get; set; }
            public string Expires_in { get; set; }
            public string Token_type { get; set; }

        }
    }
}
