using System;
using System.Collections.Concurrent;
using System.Linq;
using Jellyfin.Plugin.Streamyfin.Configuration;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.Events;

public abstract class EventCache
{
    protected static readonly ConcurrentDictionary<string, DateTime> _recentEvents = new();
    private static readonly TimeSpan RecentEventThreshold = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan CleanupThreshold = TimeSpan.FromMinutes(5);

    protected static Config? _config => StreamyfinPlugin.Instance?.Configuration.Config;

    /// <summary>
    /// Check if the event was recently processed before
    /// </summary>
    /// <param name="sessionKey"></param>
    /// <returns></returns>
    public bool HasRecentlyProcessed(string sessionKey)
    {
        var recentlyProcessed = 
            _recentEvents.TryGetValue(sessionKey, out DateTime lastProcessedTime) && 
            DateTime.UtcNow - lastProcessedTime < GetRecentEventThreshold();

        if (!recentlyProcessed)
        {
            // Update the cache with the latest event time
            _recentEvents[sessionKey] = DateTime.UtcNow;
        }

        return recentlyProcessed;
    }
    
    /// <summary>
    /// Cleans up old session entries from the cache.
    /// </summary>
    public void CleanupOldEntries()
    {
        DateTime threshold = DateTime.UtcNow - GetCleanupThreshold();
        var keysToRemove = _recentEvents
            .Where(kvp => kvp.Value < threshold)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _recentEvents.TryRemove(key, out _);
        }
    }

    public virtual TimeSpan GetRecentEventThreshold()
    {
        return RecentEventThreshold;
    }
    
    public virtual TimeSpan GetCleanupThreshold()
    {
        return CleanupThreshold;
    }
}