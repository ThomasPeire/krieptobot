using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KrieptoBot.Infrastructure.Bitvavo
{
    public class BitvavoAuthHeaderHandler : DelegatingHandler
    {
        private readonly BitvavoConfig _bitvavoConfig;

        public BitvavoAuthHeaderHandler(IOptions<BitvavoConfig> bitvavoConfigOptions)
        {
            _bitvavoConfig = bitvavoConfigOptions.Value;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var url = request.RequestUri.AbsolutePath + request.RequestUri.Query;
            var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            const string httpMethod = "GET";
            const string body = "";

            var toHash = timeStamp + httpMethod + url + body;
            var signature = GenerateHeaderSignature(toHash, _bitvavoConfig.ApiSecret);

            request.Headers.Add("Bitvavo-Access-Key", _bitvavoConfig.ApiKey);
            request.Headers.Add("Bitvavo-Access-Window", "20000");
            request.Headers.Add("Bitvavo-Access-Timestamp", timeStamp);
            request.Headers.Add("Bitvavo-Access-Signature", signature);

            return base.SendAsync(request, cancellationToken);
        }

        private string GenerateHeaderSignature(string toHash, string apiSecret)
        {
            var encoding = new UTF8Encoding();

            var textBytes = encoding.GetBytes(toHash);
            var keyBytes = encoding.GetBytes(apiSecret);

            byte[] hashBytes;

            using (var hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(CultureInfo.InvariantCulture);
        }
    }

}
