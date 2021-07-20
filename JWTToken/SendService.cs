using Dalion.HttpMessageSigning;
using Dalion.HttpMessageSigning.Signing;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace JWTToken
{
    public class SendService
    {
        private readonly HttpClient httpclient;

          public SendService(HttpClient httpclient)
        {
            this.httpclient = httpclient;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await httpclient.SendAsync(request, cancellationToken);
        }
    }

}
