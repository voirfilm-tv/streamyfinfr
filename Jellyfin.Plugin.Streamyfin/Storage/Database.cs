using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Data.Sqlite;
using Jellyfin.Plugin.Streamyfin.Storage.Enums;
using Newtonsoft.Json;

namespace Jellyfin.Plugin.Streamyfin.Storage;

public class DeviceToken
{
    [JsonProperty(PropertyName = "token")]
    public string Token { get; set; }
    [JsonProperty(PropertyName = "deviceId")]
    public Guid DeviceId { get; set; }
    [JsonProperty(PropertyName = "userId")]
    public Guid UserId { get; set; }
    [JsonProperty(PropertyName = "timestamp")]
    public long Timestamp { get; set; }
}

public class Database : IDisposable
{
    private readonly string name = "streamyfin_plugin.db";
    private bool _disposed = false;
    protected ReaderWriterLockSlim WriteLock { get; }

    public string DbFilePath { get; set; }

    protected virtual int? CacheSize => null;

    protected virtual string LockingMode => "NORMAL";

    protected virtual string JournalMode => "WAL";

    protected virtual int? JournalSizeLimit => 134_217_728; // 128MiB

    protected virtual int? PageSize => null;

    protected virtual TempStoreMode TempStore => TempStoreMode.Memory;
    
    private const string DeviceTokensTable = "device_tokens";

    public Database(string path)
    {
        Directory.CreateDirectory(path);
        DbFilePath = Path.Combine(path, name);
        Initialize(File.Exists(DbFilePath));
        WriteLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    }

    public void Initialize(bool fileExists)
    {
        using (var connection = CreateConnection())
        {
            string[] queries =
            {
                $"create table if not exists {DeviceTokensTable} (DeviceId GUID PRIMARY KEY, Token TEXT NOT NULL, UserId GUID NOT NULL, Timestamp INTEGER NOT NULL)",
                $"create index if not exists idx_{DeviceTokensTable}_user on {DeviceTokensTable}(UserId)",
            };

            connection.RunQueries(queries);
        }
    }

    public List<DeviceToken> GetUserDeviceTokens(Guid userId)
    {
        using (WriteLock.Read())
        {
            using (var connection = CreateConnection(true))
            {
                List<DeviceToken> tokens = new List<DeviceToken>();
                using (var statement = connection.PrepareStatement($"select * from {DeviceTokensTable} where UserId = @UserId;"))
                {
                    statement.TryBind("@UserId", userId);
                    
                    foreach (var row in statement.ExecuteQuery())
                    {
                        tokens.Add(
                            new DeviceToken
                            {
                                DeviceId = row.GetGuid(0),
                                Token = row.GetString(1),
                                UserId = row.GetGuid(2),
                                Timestamp = row.GetInt64(3)
                            }
                        );
                    }
                }

                return tokens;
            }
        }
    }
    
    public List<DeviceToken> GetAllDeviceTokens()
    {
        using (WriteLock.Read())
        {
            using (var connection = CreateConnection(true))
            {
                List<DeviceToken> tokens = new List<DeviceToken>();
                using (var statement = connection.PrepareStatement($"select * from {DeviceTokensTable};"))
                {
                    foreach (var row in statement.ExecuteQuery())
                    {
                        tokens.Add(
                            new DeviceToken
                            {
                                DeviceId = row.GetGuid(0),
                                Token = row.GetString(1),
                                UserId = row.GetGuid(2),
                                Timestamp = row.GetInt64(3)
                            }
                        );
                    }
                }

                return tokens;
            }
        }
    }

    public DeviceToken GetDeviceTokenForDeviceId(Guid deviceId)
    {
        using (WriteLock.Read())
        {
            using (var connection = CreateConnection(true))
            {
                using (var statement =
                       connection.PrepareStatement($"select * from {DeviceTokensTable} where DeviceId = @DeviceId;"))
                {
                    statement.TryBind("@DeviceId", deviceId);
                    foreach (var row in statement.ExecuteQuery())
                    {
                        return new DeviceToken
                        {
                            DeviceId = row.GetGuid(0),
                            Token = row.GetString(1),
                            UserId = row.GetGuid(2),
                            Timestamp = row.GetInt64(3)
                        };
                    }

                    return null;
                }
            }
        }
    }

    public DeviceToken AddDeviceToken(DeviceToken token)
    {
        using (WriteLock.Write())
        {
            using (var connection = CreateConnection(true))
            {
                return connection.RunInTransaction(db =>
                {
                    long timestamp = DateTime.UtcNow.ToFileTime();

                    using (var statement = db.PrepareStatement($"delete from {DeviceTokensTable} where DeviceId=@DeviceId;"))
                    {
                        statement.TryBind("@DeviceId", token.DeviceId);
                        statement.ExecuteNonQuery();
                    }

                    using (var statement = db.PrepareStatement($"insert into {DeviceTokensTable}(DeviceId, Token, UserId, Timestamp) values (@DeviceId, @Token, @UserId, @Timestamp);"))
                    {
                        statement.TryBind("@DeviceId", token.DeviceId);
                        statement.TryBind("@Token", token.Token);
                        statement.TryBind("@UserId", token.UserId);
                        statement.TryBind("@Timestamp", timestamp);
                        statement.ExecuteNonQuery();
                    }

                    token.Timestamp = timestamp;
                    return token;
                });
            }
        }
    }

    public void RemoveDeviceToken(Guid deviceId)
    {
        using (WriteLock.Write())
        {
            using (var connection = CreateConnection())
            {
                connection.RunInTransaction(db =>
                {
                    using (var statement = db.PrepareStatement($"delete from {DeviceTokensTable} where DeviceId=@DeviceId;"))
                    {
                        statement.TryBind("@DeviceId", deviceId);
                        statement.ExecuteNonQuery();
                    }
                });
            }
        }
    }

    protected SqliteConnection CreateConnection(bool isReadOnly = false)
    {
        var connection = new SqliteConnection($"Filename={DbFilePath}");
        connection.Open();

        if (CacheSize.HasValue)
        {
            connection.Execute("PRAGMA cache_size=" + CacheSize.Value);
        }

        if (!string.IsNullOrWhiteSpace(LockingMode))
        {
            connection.Execute("PRAGMA locking_mode=" + LockingMode);
        }

        if (!string.IsNullOrWhiteSpace(JournalMode))
        {
            connection.Execute("PRAGMA journal_mode=" + JournalMode);
        }

        if (JournalSizeLimit.HasValue)
        {
            connection.Execute("PRAGMA journal_size_limit=" + JournalSizeLimit.Value);
        }

        if (PageSize.HasValue)
        {
            connection.Execute("PRAGMA page_size=" + PageSize.Value);
        }

        connection.Execute("PRAGMA temp_store=" + (int)TempStore);

        return connection;
    }

    public void Purge()
    {
        using (WriteLock.Write())
        {
            using (var connection = CreateConnection(true))
            {
                connection.RunInTransaction(db =>
                {
                    using (var statement = db.PrepareStatement($"delete from {DeviceTokensTable};"))
                    {
                        statement.ExecuteNonQuery();
                    }
                });
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}