namespace Microsoft.Azure.Cosmos;

/// <summary>
/// Extension methods used in this project on CosmosClient.
/// </summary>
public static class CosmosExtensions
{
    /// <summary>
    /// Get a container from a given database. This method ensure that the 
    /// database and container exist, and also allows for properties like
    /// default time to live to be easily configured.
    /// </summary>
    /// <param name="cosmosClient">The cosmos client being extended</param>
    /// <param name="databaseId">The ID for the database</param>
    /// <param name="containerId">The ID for the container</param>
    /// <param name="partitionKeyPath">The container's partition key path</param>
    /// <param name="defaultTimeToLive">The container's document's TTL</param>
    /// <returns></returns>
    public static async Task<Container> UseContainer(
        this CosmosClient cosmosClient,
        string databaseId,
        string containerId,
        string partitionKeyPath = "/id",
        int defaultTimeToLive = -1)
    {
        DatabaseResponse databaseResponse = await cosmosClient
            .CreateDatabaseIfNotExistsAsync(databaseId);

        ContainerProperties properties = new()
        {
            Id = containerId,
            PartitionKeyPath = partitionKeyPath,
            DefaultTimeToLive = defaultTimeToLive
        };

        ContainerResponse containerResponse = await databaseResponse.Database
            .CreateContainerIfNotExistsAsync(properties);

        return containerResponse.Container;
    }
}