using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Jellyfin.Plugin.Streamyfin.Storage;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Assert = ICU4N.Impl.Assert;

namespace Jellyfin.Plugin.Streamyfin.tests;

/// <summary>
/// Run before and after every db test
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CleanupDatabaseBeforeAndAfter: BeforeAfterTestAttribute
{
    private readonly Database db = new(Directory.GetCurrentDirectory());

    public override void Before(MethodInfo methodUnderTest)
    {
        db.Purge();
    }

    public override void After(MethodInfo methodUnderTest)
    {
        db.Purge();
    }
}

/// <summary>
/// Ensure [Jellyfin.Plugin.Streamyfin.Storage.Database] can properly run transactions as expected
/// </summary>
public class DatabaseTests(ITestOutputHelper output): IDisposable
{
    private readonly Database db = new(Directory.GetCurrentDirectory());
    private readonly SerializationHelper _serializationHelper = new();

    /// <summary>
    /// Ensure when adding a device token for a specific device that we delete any previous old token first 
    /// </summary>
    [Fact]
    [CleanupDatabaseBeforeAndAfter]
    public void TestAddingDeviceTokenForTheSameDevice()
    {
        var token = db.AddDeviceToken(
            new DeviceToken
            {
                DeviceId = "testId",
                Token = "testToken",
                UserId = "testUserId"
            }
        );
        
        // Adding a "new" token should update the timestamp
        var updatedToken = db.AddDeviceToken(token);

        var newTokenReference = db.GetDeviceTokenForDeviceId(token.DeviceId);
        
        Assert.Assrt(
            $"Timestamp was updated",
            updatedToken.Timestamp != newTokenReference.Timestamp
        );
    }
    
    /// <summary>
    /// Make sure we are actually recording each token 
    /// </summary>
    [Fact]
    [CleanupDatabaseBeforeAndAfter]
    public void TestAllTokensPersistSeparately()
    {
        for (int i = 0; i < 5; i++)
        {
            db.AddDeviceToken(
                new DeviceToken
                {
                    DeviceId = $"device{i.ToString(CultureInfo.InvariantCulture)}",
                    Token = $"token{i.ToString(CultureInfo.InvariantCulture)}",
                    UserId = "testUserId"
                }
            );
        }

        var tokens = db.GetAllDeviceTokens();

        Assert.Assrt(
            $"All tokens persisted",
            tokens.Count == 5
        );
    }

    public void Dispose()
    {
        output.WriteLine($"Deleting database {db.DbFilePath}");
        File.Delete(db.DbFilePath);
    }
}