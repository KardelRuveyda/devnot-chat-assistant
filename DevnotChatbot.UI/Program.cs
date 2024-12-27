using DevnotChatbot.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7039/") });

await builder.Build().RunAsync();
