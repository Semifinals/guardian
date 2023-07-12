using Semifinals.Guardian.Repositories;

namespace Semifinals.Guardian.Services;

public interface IAuthenticationService
{
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IIntegrationRepository _integrationRepository;

    public AuthenticationService(
        IAccountRepository accountRepository,
        IIdentityRepository identityRepository,
        IIntegrationRepository integrationRepository)
    {
        _accountRepository = accountRepository;
        _identityRepository = identityRepository;
        _integrationRepository = integrationRepository;
    }
}
