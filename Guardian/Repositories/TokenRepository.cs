using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Repositories;

public interface ITokenRepository
{
    /// <summary>
    /// Create a new token.
    /// </summary>
    /// <param name="identityId">The ID of the identity the token is for</param>
    /// <param name="token">The token</param>
    /// <param name="iat">The type of recovery it can perform</param>
    /// <returns>The newly created recovery code</returns>
    Task<Token> CreateAsync(string identityId, string token, int iat);
    
    /// <summary>
    /// Fetch a token to see if it exists and can be used.
    /// </summary>
    /// <param name="identityId">The ID of the identity the token is for</param>
    /// <param name="iat">The unix timestamp for when the token was generated</param>
    /// <returns>The requested token if it exists</returns>
    /// <exception cref="TokenNotFoundException">Occurs when the token does not exist</exception>
    Task<Token> GetByIdAsync(string identityId, int iat);

    /// <summary>
    /// Delete a recovery code by the given ID.
    /// </summary>
    /// <param name="identityId">The ID of the identity the token is for</param>
    /// <param name="iat">The unix timestamp for when the token was generated</param>
    /// <returns>A task which resolves when the token is deleted</returns>
    Task DeleteByIdAsync(string identityId, int iat);
}

public class TokenRepository : ITokenRepository
{
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmosClient;

    private Task<Container> GetTokenContainer()
    {
        return _cosmosClient.UseContainer(
            "identity-db",
            "tokens",
            partitionKeyPath: "/identityId",
            defaultTimeToLive: 86400 * 14);
    }

    public TokenRepository(ILogger logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    public async Task<Token> CreateAsync(
        string identityId,
        string token,
        int iat)
    {
        Container container = await GetTokenContainer();

        Token refreshToken = await container.UpsertItemAsync<Token>(
            new(identityId, token, iat),
            new(identityId));
        
        _logger.LogInformation("Created new refresh token for ID {id}", identityId);

        return refreshToken;
    }

    public async Task<Token> GetByIdAsync(string identityId, int iat)
    {
        Container container = await GetTokenContainer();
        
        Token refreshToken;
        try
        {
            refreshToken = await container.ReadItemAsync<Token>(
                Token.GetCompositeId(identityId, iat),
                new(identityId));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to fetch a token for ID {id}",
                identityId);

            throw new TokenNotFoundException(identityId, iat);
        }

        _logger.LogInformation(
               "Successfully fetched a token for ID {id}",
               identityId);

        return refreshToken;
    }

    public async Task DeleteByIdAsync(string identityId, int iat)
    {
        Container container = await GetTokenContainer();
        
        await container.DeleteItemAsync<Token>(
            Token.GetCompositeId(identityId, iat),
            new(identityId));

        _logger.LogInformation(
            "Deleted a token for ID {id}",
            identityId);
    }
}
