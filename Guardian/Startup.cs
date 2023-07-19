using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Semifinals.Guardian.Repositories;

[assembly: FunctionsStartup(typeof(Semifinals.Guardian.Startup))]

namespace Semifinals.Guardian;

class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(
        IFunctionsConfigurationBuilder builder)
    {
        // TODO: Override with Azure Key Vault configuration

        base.ConfigureAppConfiguration(builder);
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Setup access to configuration
        IConfiguration config = builder.GetContext().Configuration;
        builder.Services.AddSingleton(config);

        // Setup logging
        builder.Services.AddLogging();

        // Setup document database
        builder.Services.AddSingleton<CosmosClient>(provider =>
        {
            return new(
                accountEndpoint: config["DOCUMENT_DB_ENDPOINT"],
                authKeyOrResourceToken: config["DOCUMENT_DB_KEY"]);
        });

        // Setup repositories
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
        builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();
        builder.Services.AddScoped<IRecoveryCodeRepository, RecoveryCodeRepository>();

        // Setup services
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
