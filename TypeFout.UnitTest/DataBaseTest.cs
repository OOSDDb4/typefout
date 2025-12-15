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