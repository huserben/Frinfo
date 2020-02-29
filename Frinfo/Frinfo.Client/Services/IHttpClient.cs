using System.Net.Http;
using System.Threading.Tasks;

namespace Frinfo.Client.Services
{
   public interface IHttpClient
   {
      Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption);

      Task<HttpResponseMessage> DeleteAsync(string requestUri);

      Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
   }
}
