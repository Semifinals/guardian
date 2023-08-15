namespace Semifinals.Guardian.Models;

[TestClass]
public class TokenTests
{
    [TestMethod]
    public void GenerateCompositeId_GeneratesCorrectly()
    {
        // Arrange
        string identityId = "identityId";
        int iat = 1234567890;
        
        // Act
        string compositeId = Token.GetCompositeId(identityId, iat);
        
        // Assert
        Assert.AreEqual("identityId:1234567890", compositeId);
    }
}
