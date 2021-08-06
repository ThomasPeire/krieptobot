using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KrieptoBod.Exchange.Bitvavo
{
    internal class Client<T> : IClient<T>
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _baseUrl;

        public Client(string apiKey, string apiSecret, string baseUrl)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _baseUrl = baseUrl;
        }

        private HttpClient AddHeaders(HttpClient client, string timeStamp, string accessWindow, string signature)
        {
            client.DefaultRequestHeaders.Add("Bitvavo-Access-Key", this._apiKey);
            client.DefaultRequestHeaders.Add("Bitvavo-Access-Timestamp", timeStamp);
            client.DefaultRequestHeaders.Add("Bitvavo-Access-Window", accessWindow);
            client.DefaultRequestHeaders.Add("Bitvavo-Access-Signature", signature);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private string GenerateHeaderSignature(string toHash)
        {
            var encoding = new UTF8Encoding();

            var textBytes = encoding.GetBytes(toHash);
            var keyBytes = encoding.GetBytes(this._apiSecret);

            byte[] hashBytes;

            using (var hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<T> GetAsync(string url)
        {
            var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            const string accessWindow = "20000";
            const string httpMethod = "GET";
            const string body = "";

            var toHash = timeStamp + httpMethod + url + body;
            var signature = GenerateHeaderSignature(toHash);

            using var client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };

            AddHeaders(client, timeStamp, accessWindow, signature);

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseBody = await response.Content.ReadAsStringAsync();

            return await JsonSerializer.DeserializeAsync<T>(responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
