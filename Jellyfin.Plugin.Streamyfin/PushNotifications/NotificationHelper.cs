using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Jellyfin.Plugin.Streamyfin.PushNotifications.models;

namespace Jellyfin.Plugin.Streamyfin.PushNotifications;

public class NotificationHelper
{
    private readonly SerializationHelper _serializationHelper;
    
    public NotificationHelper(SerializationHelper serializationHelper)
    {
        _serializationHelper = serializationHelper;
    }

    public async Task<ExpoNotificationResponse?> send(List<ExpoNotificationRequest> requests) =>
        await SendNotificationToExpo(_serializationHelper.ToJson(requests));
    
    public async Task<ExpoNotificationResponse?> send(ExpoNotificationRequest request) =>
        await SendNotificationToExpo(_serializationHelper.ToJson(request));

    private async Task<ExpoNotificationResponse?> SendNotificationToExpo(string serializedRequest)
    {
        using HttpClient client = new();
        var httpRequest = GetHttpRequestMessage(serializedRequest);
        var rawResponse = await client.SendAsync(httpRequest);
        return await rawResponse.Content.ReadFromJsonAsync<ExpoNotificationResponse>().ConfigureAwait(true);
    }

    private HttpRequestMessage GetHttpRequestMessage(string content) => new()
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri("https://exp.host/--/api/v2/push/send"),
        Headers =
        {
            { "Host", "exp.host" },
            { "Accept", "application/json" },
            { "Accept-Encoding", "gzip, deflate" }
        },
        Content = new StringContent(
            content: content,
            encoding: Encoding.UTF8,
            mediaType: "application/json"
        )
    };
}