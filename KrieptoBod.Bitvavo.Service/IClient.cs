using System.Net.Http;
using System.Threading.Tasks;

namespace KrieptoBod.Exchange
{
    public interface IClient
    {
        Task<HttpContent> GetAsync(string url);
    }
}
