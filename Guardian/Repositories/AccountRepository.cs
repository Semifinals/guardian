using Semifinals.Guardian.Models;

namespace Semifinals.Guardian.Repositories;

public interface IAccountRepository
{
    /// <summary>
    /// Create a new first-party account. Provide the identity's ID if the
    /// identity already exists from registration through an integration.
    /// </summary>
    /// <param name="emailAddress">The unique email address for the account</param>
    /// <param name="passwordHashed">The account's password after being hashed</param>
    /// <param name="id">The ID of the identity if it already exists</param>
    /// <returns></returns>
    Task<Account> CreateAsync(
        string emailAddress,
        string passwordHashed,
        string? id = null);

    /// <summary>
    /// Get an account by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to fetch</param>
    /// <returns>The associated account</returns>
    Task<Account?> GetByIdAsync(string id);

    /// <summary>
    /// Update an account by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to update</param>
    /// <param name="operations">The patch operations to perform</param>
    /// <returns>The updated account</returns>
    Task<Account?> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations);

    /// <summary>
    /// Delete an account by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to delete</param>
    /// <returns>A task which resolves when the account is deleted</returns>
    Task DeleteByIdAsync(string id);
}

public class AccountRepository : IAccountRepository
{
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmosClient;

    private Container Container => _cosmosClient.GetContainer(
        "identity-db",
        "accounts");

    public AccountRepository(ILogger logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }
    
    public async Task<Account> CreateAsync(
        string emailAddress,
        string passwordHashed,
        string? id = null)
    {
        id ??= ShortId.Generate(new(useSpecialCharacters: false, length: 8));

        Account account = await Container.CreateItemAsync<Account>(
            new(id, emailAddress, passwordHashed, false),
            new(id));

        _logger.LogInformation("Created new account with ID {id}", id);

        return account;
    }

    public async Task<Account?> GetByIdAsync(string id)
    {
        Account account;
        try
        {
            account = await Container.ReadItemAsync<Account>(id, new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to get the account with ID {id}",
                id);

            return null;
        }

        _logger.LogInformation("Fetched the account with ID {id}", id);
        
        return account;
    }
    
    public async Task<Account?> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations)
    {
        Account account;
        try
        {
            account = await Container.PatchItemAsync<Account>(
                id,
                new(id),
                (IReadOnlyList<PatchOperation>)operations);
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to update the account with ID {id} changing the following properties: {properties}",
                id,
                string.Join(", ", operations.Select(o => o.Path)));

            return null;
        }

        _logger.LogInformation(
            "Updated the account with ID {id} changing the following properties: {properties}",
            id,
            string.Join(", ", operations.Select(o => o.Path)));

        return account;
    }

    public async Task DeleteByIdAsync(string id)
    {
        await Container.DeleteItemAsync<Account>(id, new(id));

        _logger.LogInformation("Deleted the account with ID {id}", id);
    }
}
