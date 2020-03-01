using System.Threading.Tasks;
using Blazor.FileReader;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Frinfo.Client.Services;
using Caliburn.Micro;
using Blazored.Toast;

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
         services.AddBlazoredToast();

         services.AddScoped<IHouseholdDataService, HouseholdDataService>();
         services.AddScoped<IFridgeDataService, FridgeDataService>();

         services.AddSingleton<IHttpClient, FrinfoHttpClient>();
         services.AddScoped<ILocalStorageHouseholdService, LocalStorageHouseholdService>();

         services.AddSingleton<IEventAggregator, EventAggregator>();

         services.AddBlazoredLocalStorage();
         services.AddFileReaderService(options => {
            options.UseWasmSharedBuffer = true;
         });
      }
   }
}
