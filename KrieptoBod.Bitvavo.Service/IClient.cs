using System.Threading.Tasks;

namespace KrieptoBod.Exchange
{
    internal interface IClient<T>
    {
        Task<T> GetAsync(string url);
    }
}
