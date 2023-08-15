namespace Semifinals.Guardian.DTOs;

[TestClass]
public class AccountAuthDtoTests
{
    public readonly AccountAuthDtoValidator Validator = new();
    
    public static AccountAuthDto Deserialize(string json) =>
        JsonSerializer.Deserialize<AccountAuthDto>(json)!;
    
    [TestMethod]
    public void EmailAddress_AcceptsValid()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""emailAddress"": ""user@example.com"",
            ""password"": ""Example#12""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    [DataRow("invalid")]
    [DataRow("example.com")]
    [DataRow("user@")]
    public void EmailAddress_RejectsInvalid(string invalid)
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""emailAddress"": ""{invalid}"",
            ""password"": ""Example#12""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void EmailAddress_RejectsNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""password"": ""Example#12""
        }}");
        
        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Password_AcceptsValid()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""emailAddress"": ""user@example.com"",
            ""password"": ""Example#12""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    [DataRow("password#12")]
    [DataRow("PASSWORD#12")]
    [DataRow("Password#")]
    [DataRow("Password12")]
    public void Password_RejectsInvalid(string invalid)
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""emailAddress"": ""user@example.com"",
            ""password"": ""{invalid}""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Password_RejectsNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""emailAddress"": ""user@example.com""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }
}
