using System.Net.Http;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public interface IHttpClient
   {
      bool IsOnline { get; }

      Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption);

      Task<HttpResponseMessage> DeleteAsync(string requestUri);

      Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
   }
}
