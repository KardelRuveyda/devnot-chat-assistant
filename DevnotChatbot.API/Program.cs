using DevnotChatbot.API.Models.Constants;
using DevnotChatbot.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// appsettings.json ve secrets.json dosyalarýný yükleme
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // appsettings.json'ý yükler
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true) // Ortam spesifik ayarlarý yükler
    .AddUserSecrets<Program>(optional: true, reloadOnChange: true); // Geliþtirme ortamýnda secrets.json'ý yükler

// CORS ekleme
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorFrontend", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:7187") // Blazor projenizin URL'si
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

#region Configuration   

// OpenAIConstants ayarlarýný configure etme
builder.Services.AddOptions<OpenAIConstants>()
    .Bind(builder.Configuration.GetSection("OpenAIConstants")) // OpenAIConstants sekmesinden baðlama
    .ValidateDataAnnotations() // Veri doðrulamasý yapma
    .Validate(settings => !string.IsNullOrEmpty(settings.ApiKey) && !string.IsNullOrEmpty(settings.FilePath),
              "Configuration settings are missing or incomplete. Please check appsettings.json or secrets.json.")
    .Configure(options =>
    {
        // Loglama veya debug iþlemleri yapýlabilir
        Console.WriteLine($"ApiKey: {options.ApiKey}, FilePath: {options.FilePath}");
    });

#endregion

// Hizmetlerin eklenmesi
builder.Services.AddScoped<IDevnotChatbotService, DevnotChatbotService>(); // Servis ekleme

// Controller'larý ekleyin
builder.Services.AddControllers();

// Swagger ekleme
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// CORS politikasýný kullan
app.UseCors("AllowBlazorFrontend");
app.UseHttpsRedirection();

// Authorization kullanmýyorsanýz bu satýrý kaldýrabilirsiniz
// app.UseAuthorization();

app.MapControllers(); // Controller'larýn çalýþabilmesi için gerekli

app.Run();
