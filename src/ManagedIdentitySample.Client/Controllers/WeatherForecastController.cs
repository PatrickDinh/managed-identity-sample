using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ManagedIdentitySample.Client.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ManagedIdentitySample.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _appSettings;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettingsOptions)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _appSettings = appSettingsOptions.Value;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            using var httpClient = _httpClientFactory.CreateClient(Startup.WeatherApiHttpClientName);
            
            var response = await httpClient.GetAsync($"{_appSettings.WeatherBaseApi}/WeatherForecast");
            response.EnsureSuccessStatusCode();
            
            return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}