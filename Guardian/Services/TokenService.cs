using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;

public interface ITokenService
{
    /// <summary>
    /// Genereate an access token with an optional refresh token.
    /// </summary>
    /// <param name="id">The ID of the user being authenticated</param>
    /// <param name="withRefreshToken">Whether or not a refresh token should be generated</param>
    /// <returns>A token response with the generated tokens</returns>
    Task<TokenResponse> GenerateAccessToken(
        string id,
        bool withRefreshToken = false);
}

public class TokenService : ITokenService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly ITokenRepository _tokenRepository;
    
    public TokenService(
        ILogger logger,
        IConfiguration configuration,
        ITokenRepository tokenRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _tokenRepository = tokenRepository;
    }

    public async Task<TokenResponse> GenerateAccessToken(
        string id,
        bool withRefreshToken = false)
    {
        long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        string accessToken = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_configuration.GetValue<string>("JWT_SECRET"))
            .AddClaim("sub", id)
            .AddClaim("exp", currentTimestamp + 3600)
            .AddClaim("iat", currentTimestamp)
            .Encode();

        string? refreshToken = null;
        if (withRefreshToken)
        {
            string refreshTokenJti = ShortId.Generate(
                new(useSpecialCharacters: false, length: 8));
            
            refreshToken = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_configuration.GetValue<string>("JWT_SECRET"))
                .AddClaim("sub", id)
                .AddClaim("exp", currentTimestamp + 86400 * 7)
                .AddClaim("iat", currentTimestamp)
                .AddClaim("jti", refreshTokenJti)
                .Encode();

            await _tokenRepository.CreateAsync(id, refreshToken, (int)currentTimestamp);
        }

        _logger.LogInformation(
            $"Generated a new access token {(withRefreshToken ? " and refresh token" : "")} for user {id}");

        return new(accessToken, refreshToken, 3600, "Bearer");
    }
}
