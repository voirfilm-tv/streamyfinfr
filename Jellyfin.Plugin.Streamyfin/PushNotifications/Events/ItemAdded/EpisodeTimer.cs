using System;
using System.Collections.Generic;
using System.Threading;
using MediaBrowser.Controller.Entities.TV;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications.Events.ItemAdded;

public class EpisodeTimer
{
    public List<Episode> Episodes { get; set; }
    private Timer Timer { get; set; }

    public EpisodeTimer(
        List<Episode> episodes,
        TimerCallback callback)
    {
        Episodes = episodes;
        Timer = new Timer(
            callback: callback,
            state: null,
            dueTime: Timeout.InfiniteTimeSpan,
            period: Timeout.InfiniteTimeSpan
        );
    }

    public void Add(Episode episode)
    {
        ResetTimer();
        var index = Episodes.FindIndex(e => e.Id == episode.Id);

        if (index == -1)
        {
            Episodes.Add(episode);
        }
        else
        {
            Episodes[index] = episode;
        }
    }

    private void ResetTimer() => Timer.Change(TimeSpan.FromSeconds(60), Timeout.InfiniteTimeSpan);
};