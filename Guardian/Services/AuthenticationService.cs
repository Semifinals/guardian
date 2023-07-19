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
    private readonly IRecoveryCodeRepository _recoveryCodeRepository;

    public AuthenticationService(
        IAccountRepository accountRepository,
        IIdentityRepository identityRepository,
        IIntegrationRepository integrationRepository,
        IRecoveryCodeRepository recoveryCodeRepository)
    {
        _accountRepository = accountRepository;
        _identityRepository = identityRepository;
        _integrationRepository = integrationRepository;
        _recoveryCodeRepository = recoveryCodeRepository;
    }
}
