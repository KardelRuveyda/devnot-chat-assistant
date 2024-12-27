using DevnotChatbot.API.Models.Constants;
using DevnotChatbot.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// appsettings.json ve secrets.json dosyalar�n� y�kleme
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // appsettings.json'� y�kler
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true) // Ortam spesifik ayarlar� y�kler
    .AddUserSecrets<Program>(optional: true, reloadOnChange: true); // Geli�tirme ortam�nda secrets.json'� y�kler

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

// OpenAIConstants ayarlar�n� configure etme
builder.Services.AddOptions<OpenAIConstants>()
    .Bind(builder.Configuration.GetSection("OpenAIConstants")) // OpenAIConstants sekmesinden ba�lama
    .ValidateDataAnnotations() // Veri do�rulamas� yapma
    .Validate(settings => !string.IsNullOrEmpty(settings.ApiKey) && !string.IsNullOrEmpty(settings.FilePath),
              "Configuration settings are missing or incomplete. Please check appsettings.json or secrets.json.")
    .Configure(options =>
    {
        // Loglama veya debug i�lemleri yap�labilir
        Console.WriteLine($"ApiKey: {options.ApiKey}, FilePath: {options.FilePath}");
    });

#endregion

// Hizmetlerin eklenmesi
builder.Services.AddScoped<IDevnotChatbotService, DevnotChatbotService>(); // Servis ekleme

// Controller'lar� ekleyin
builder.Services.AddControllers();

// Swagger ekleme
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// CORS politikas�n� kullan
app.UseCors("AllowBlazorFrontend");
app.UseHttpsRedirection();

// Authorization kullanm�yorsan�z bu sat�r� kald�rabilirsiniz
// app.UseAuthorization();

app.MapControllers(); // Controller'lar�n �al��abilmesi i�in gerekli

app.Run();
