namespace Semifinals.Guardian.Mocks;

public class CosmosClientMockBuilder
{
    private readonly Mock<CosmosClient> _cosmosClient;
    private readonly Mock<Container> _container;

    public CosmosClientMockBuilder()
    {
        _cosmosClient = new();
        _container = new();
    }

    public CosmosClientMockBuilder SetupContainer(Action<Mock<Container>> callback)
    {
        callback(_container);
        return this;
    }

    public Mock<CosmosClient> Create()
    {
        Mock<ContainerResponse> containerResponse = new();
        containerResponse.Setup(x => x.Container).Returns(_container.Object);

        Mock<Database> database = new();
        database
            .Setup(x => x.CreateContainerIfNotExistsAsync(
                It.IsAny<ContainerProperties>(),
                (ThroughputProperties?)null,
                null,
                default))
            .ReturnsAsync(containerResponse.Object);
        database.Setup(x => x.CreateContainerIfNotExistsAsync(
                It.IsAny<ContainerProperties>(),
                (int?)null,
                null,
                default))
            .ReturnsAsync(containerResponse.Object);

        Mock<DatabaseResponse> databaseResponse = new();
        databaseResponse.Setup(x => x.Database).Returns(database.Object);

        _cosmosClient
            .Setup(x => x.CreateDatabaseIfNotExistsAsync(
                It.IsAny<string>(),
                (ThroughputProperties?)null,
                null,
                default))
            .ReturnsAsync(databaseResponse.Object);
        _cosmosClient
            .Setup(x => x.CreateDatabaseIfNotExistsAsync(
                It.IsAny<string>(),
                (int?)null,
                null,
                default))
            .ReturnsAsync(databaseResponse.Object);

        return _cosmosClient;
    }
}
