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
        public HttpClient Client;
        private readonly BitvavoConfig _bitvavoConfig;

        public BitvavoClient(HttpClient client, BitvavoConfig bitvavoConfig)
        {
            client.BaseAddress = new Uri(bitvavoConfig.BaseUrl);
            Client = client;
            _bitvavoConfig = bitvavoConfig;
        }

        private void AddHeaders(string timeStamp, string accessWindow, string signature)
        {
            Client.DefaultRequestHeaders.Add("Bitvavo-Access-Key", this._bitvavoConfig.ApiKey);
            Client.DefaultRequestHeaders.Add("Bitvavo-Access-Timestamp", timeStamp);
            Client.DefaultRequestHeaders.Add("Bitvavo-Access-Window", accessWindow);
            Client.DefaultRequestHeaders.Add("Bitvavo-Access-Signature", signature);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            const string accessWindow = "20000";
            const string httpMethod = "GET";
            const string body = "";

            var toHash = timeStamp + httpMethod + url + body;
            var signature = GenerateHeaderSignature(toHash);

            AddHeaders(timeStamp, accessWindow, signature);

            var response = await Client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return response.Content;

        }
    }
}
