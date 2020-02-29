using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Frinfo.Client.Services;

namespace Frinfo
{
   public class Program
   {
      public static async Task Main(string[] args)
      {
         var builder = WebAssemblyHostBuilder.CreateDefault(args);
         builder.RootComponents.Add<Client.App>("app");

         ConfigureServices(builder.Services);

         await builder.Build().RunAsync();
      }

      private static void ConfigureServices(IServiceCollection services)
      {
         services.AddScoped<IHouseholdDataService, HouseholdDataService>();
         services.AddScoped<IFridgeDataService, FridgeDataService>();

         services.AddBlazoredLocalStorage();
      }
   }
}
