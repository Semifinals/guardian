﻿using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

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
    /// <returns>The newly created account</returns>
    /// <exception cref="AlreadyExistsException">Occurs when the account already exists</exception>
    Task<Account> CreateAsync(
        string emailAddress,
        string passwordHashed,
        string? id = null);

    /// <summary>
    /// Get an account by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to fetch</param>
    /// <returns>The associated account</returns>
    /// <exception cref="AccountNotFoundException">Occurs when the account does not exist</exception>
    Task<Account> GetByIdAsync(string id);

    /// <summary>
    /// Get an account by its email address.
    /// </summary>
    /// <param name="emailAddress">The email address of the account to fetch</param>
    /// <returns>The associated account</returns>
    /// <exception cref="AccountNotFoundException">Occurs when the account does not exist</exception>
    Task<Account> GetByEmailAddressAsync(string emailAddress);

    /// <summary>
    /// Update an account by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to update</param>
    /// <param name="operations">The patch operations to perform</param>
    /// <returns>The updated account</returns>
    /// <exception cref="AccountNotFoundException">Occurs when the account does not exist</exception>
    Task<Account> UpdateByIdAsync(
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

    private Task<Container> GetAccountContainer()
    {
        return _cosmosClient.UseContainer(
            "identity-db",
            "accounts",
            uniqueKeys: new() { "emailAddress" });
    }

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
        Container container = await GetAccountContainer();
        
        id ??= ShortId.Generate(new(useSpecialCharacters: false, length: 8));

        Account account;
        try
        {
            account = await container.CreateItemAsync<Account>(
            new(id, emailAddress, passwordHashed, false),
            new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation("Failed to create new account with ID {id}", id);

            throw new AlreadyExistsException();
        }

        _logger.LogInformation("Created new account with ID {id}", id);

        return account;
    }

    public async Task<Account> GetByIdAsync(string id)
    {
        Container container = await GetAccountContainer();
        
        Account account;
        try
        {
            account = await container.ReadItemAsync<Account>(id, new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to get the account with ID {id}",
                id);

            throw new AccountNotFoundException(id);
        }

        _logger.LogInformation("Fetched the account with ID {id}", id);
        
        return account;
    }

    public async Task<Account> GetByEmailAddressAsync(string emailAddress)
    {
        Container container = await GetAccountContainer();

        Account account;
        try
        {
            QueryDefinition query = new QueryDefinition(
                "SELECT * FROM accounts a WHERE a.emailAddress = @emailAddress")
                .WithParameter("@emailAddress", emailAddress);

            using FeedIterator<Account> feed = container.GetItemQueryIterator<Account>(query);
            
            FeedResponse<Account> response = await feed.ReadNextAsync();
            account = response.Resource.First();
        }
        catch (Exception)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to get the account with email {email}",
                emailAddress);

            throw new AccountNotFoundException(emailAddress);
        }

        _logger.LogInformation(
            "Fetched the account with email {email}",
            emailAddress);

        return account;
    }
    
    public async Task<Account> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations)
    {
        Container container = await GetAccountContainer();
        
        Account account;
        try
        {
            account = await container.PatchItemAsync<Account>(
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

            throw new AccountNotFoundException(id);
        }

        _logger.LogInformation(
            "Updated the account with ID {id} changing the following properties: {properties}",
            id,
            string.Join(", ", operations.Select(o => o.Path)));

        return account;
    }

    public async Task DeleteByIdAsync(string id)
    {
        Container container = await GetAccountContainer();
        
        await container.DeleteItemAsync<Account>(id, new(id));

        _logger.LogInformation("Deleted the account with ID {id}", id);
    }
}
