using ManagedIdentitySample.Client.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ManagedIdentitySample.Client
{
    public class Startup
    {
        public const string WeatherApiHttpClientName = "WeatherApiHttpClinet";
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(_configuration);
            services.AddControllers();
            services.AddHttpClient(WeatherApiHttpClientName, (provider, httpClient) =>
            {
                var appSettings = provider.GetService<IOptions<AppSettings>>().Value;
                
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = azureServiceTokenProvider.GetAccessTokenAsync(appSettings.AdAppId)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}