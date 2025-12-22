using Typefout.Core.Data.Services;
using Typefout.Core.Interfaces;
using TypeFout.UnitTest;

namespace TypeFout.UnitTest;

public class DataBaseTest
{
    [Fact]
    public void DatabaseConnectionTest()
    {
        // Arrange
        IDatabaseService databaseService = new DatabaseService();

        // Act
        int result = databaseService.Connect();

        // Assert
        Assert.Equal(202, result);
    }

    [Fact]
    public void DatabaseConnectionGiveNoErrorTest()
    {
        // Arrange
        IDatabaseService databaseService = new DatabaseService();

        // Act
        int result = databaseService.Connect();

        // Assert
        Assert.NotEqual(500, result);
    }

    [Fact]
    public void DatabaseCreateTest()
    {
        // Arrange
        IDatabaseService databaseService = new DatabaseService();

        // Act
        databaseService.Connect();
        databaseService.Open();
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "testWord", "UnitTest" },
            { "testInt", 123 }
        };
        int result = databaseService.Create("test", data);
        databaseService.Close();

        // Assert
        Assert.Equal(202, result);
    }

    [Fact]
    public void DatabaseCreateTableDoesNotExistTest()
    {
        // Arrange
        IDatabaseService databaseService = new DatabaseService();

        // Act
        databaseService.Connect();
        databaseService.Open();
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "testword", "hallo" },
            { "testInt", 42 }
        };
        int result = databaseService.Create("NoTable", data);
        databaseService.Close();

        // Assert
        Assert.Equal(500, result);
    }

    [Fact]
    public void Sanity_Check_ShoudlAlwayPass()
    {
        // Arrange
        int a = 2;
        int b = 3;

        // Act
        int result = a + b;

        // Assert
        Assert.Equal(5, result);
    }
}