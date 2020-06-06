namespace Cached.Demo.Net.Services
{
    using System.Threading.Tasks;

    public interface IFakeService
    {
        Task<string> FunctionGet(string key, int arg);
        Task<string> Get(string key);
    }
}