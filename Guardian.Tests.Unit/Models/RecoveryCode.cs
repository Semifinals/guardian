namespace Semifinals.Guardian.Models;

[TestClass]
public class RecoveryCodeTests
{
    [TestMethod]
    public void GenerateCompositeId_GeneratesCorrectly()
    {
        // Arrange
        string identityId = "identityId";
        string type = "type";

        // Act
        string compositeId = RecoveryCode.GetCompositeId(identityId, type);

        // Assert
        Assert.AreEqual("type:identityId", compositeId);
    }
}
