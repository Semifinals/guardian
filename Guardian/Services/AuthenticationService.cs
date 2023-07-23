using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;

namespace Semifinals.Guardian.Services;

public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user through a first-party account. Doing so will send a 
    /// confirmation email.
    /// </summary>
    /// <param name="emailAddress">The user's email address</param>
    /// <param name="password">The user's password</param>
    /// <returns>The newly created account</returns>
    Task<Account?> RegisterWithAccountAsync(string emailAddress, string password);

    /// <summary>
    /// Register a new user through a third-party integration.
    /// </summary>
    /// <param name="platformId">The user's unique ID on the other platform</param>
    /// <param name="platform">The name of the platform registering through</param>
    /// <returns>The newly created integration</returns>
    Task<Integration?> RegisterWithIntegrationAsync(string platformId, string platform);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IIntegrationRepository _integrationRepository;

    public AuthenticationService(
        ILogger logger,
        IAccountRepository accountRepository,
        IIdentityRepository identityRepository,
        IIntegrationRepository integrationRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _identityRepository = identityRepository;
        _integrationRepository = integrationRepository;
    }

    public async Task<Account?> RegisterWithAccountAsync(
        string emailAddress,
        string password)
    {
        throw new NotImplementedException();
    }

    public async Task<Integration?> RegisterWithIntegrationAsync(
        string platformId,
        string platform)
    {
        throw new NotImplementedException();
    }
}
