namespace Semifinals.Guardian.Utils;

[TestClass]
public class CryptoTests
{
    [TestMethod]
    public void Hash_HashesValue()
    {
        // Arrange
        string value = "test";

        // Act
        string hash = Crypto.Hash(value);

        // Assert
        Assert.IsNotNull(hash);
    }

    [TestMethod]
    public void Verify_VerifiesValid()
    {
        // Arrange
        string value = "test";
        string hash = Crypto.Hash(value);

        // Act
        bool valid = Crypto.Verify(value, hash);

        // Assert
        Assert.IsTrue(valid);
    }

    [TestMethod]
    public void Verify_DoesNotVerifyInvalid()
    {
        // Arrange
        string value = "test";
        string hash = Crypto.Hash(value);

        // Act
        bool valid = Crypto.Verify("notcorrect", hash);
        
        // Assert
        Assert.IsFalse(valid);
    }
}
