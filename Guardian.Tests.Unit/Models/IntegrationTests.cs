namespace Semifinals.Guardian.Models;

[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public void GenerateCompositeId_GeneratesCorrectly()
    {
        // Arrange
        string platform = "platform";
        string userId = "userId";

        // Act
        string compositeId = Integration.GetCompositeId(platform, userId);

        // Assert
        Assert.AreEqual("platform:userId", compositeId);
    }
}
