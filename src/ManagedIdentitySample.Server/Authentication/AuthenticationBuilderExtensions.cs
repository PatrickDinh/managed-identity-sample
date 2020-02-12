using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ManagedIdentitySample.Server.Authentication
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder ConfigureAuth(this AuthenticationBuilder builder, Action<AuthOptions> authOptions)
        {
            builder.Services.Configure(authOptions);
            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
            builder.AddJwtBearer();
            return builder;
        }

        private class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
        {
            private readonly AuthOptions _azureOptions;

            public ConfigureJwtBearerOptions(IOptions<AuthOptions> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, JwtBearerOptions options)
            {
                var azureAdRootUrl = "https://login.microsoftonline.com/";
                options.Authority = $"{azureAdRootUrl}{_azureOptions.TenantId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = new[]
                    {
                        _azureOptions.AppId
                    },
                    ValidIssuers = new[]
                    {
                        $"{azureAdRootUrl}{_azureOptions.TenantId}/v2.0"
                    }
                };
            }

            public void Configure(JwtBearerOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
