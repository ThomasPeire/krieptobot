using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KrieptoBod.Exchange.Bitvavo
{
    public class BitvavoClient
    {
        private readonly HttpClient _client;
        private readonly BitvavoConfig _bitvavoConfig;

        public BitvavoClient(HttpClient client, BitvavoConfig bitvavoConfig)
        {
            client.BaseAddress = new Uri(bitvavoConfig.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Bitvavo-Access-Key", bitvavoConfig.ApiKey);
            client.DefaultRequestHeaders.Add("Bitvavo-Access-Window", "20000");

            _client = client;
            _bitvavoConfig = bitvavoConfig;
        }

        private void AddHeaders(string timeStamp, string signature)
        {
            _client.DefaultRequestHeaders.Remove("Bitvavo-Access-Timestamp");
            _client.DefaultRequestHeaders.Remove("Bitvavo-Access-Signature");

            _client.DefaultRequestHeaders.Add("Bitvavo-Access-Timestamp", timeStamp);
            _client.DefaultRequestHeaders.Add("Bitvavo-Access-Signature", signature);
        }

        private string GenerateHeaderSignature(string toHash)
        {
            var encoding = new UTF8Encoding();

            var textBytes = encoding.GetBytes(toHash);
            var keyBytes = encoding.GetBytes(_bitvavoConfig.ApiSecret);

            byte[] hashBytes;

            using (var hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<HttpContent> GetAsync(string url)
        {
            var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            const string httpMethod = "GET";
            const string body = "";

            var toHash = timeStamp + httpMethod + url + body;
            var signature = GenerateHeaderSignature(toHash);

            AddHeaders(timeStamp, signature);

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return response.Content;

        }
    }
}
