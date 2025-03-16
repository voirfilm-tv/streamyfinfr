using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Data.Enums;
using Jellyfin.Plugin.Streamyfin.Storage.Models;
using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.Streamyfin.Extensions;

public static class UserManagerExtensions
{
    public static List<DeviceToken> GetAdminDeviceTokens(this IUserManager? manager) => (
        manager?.Users
            .Where(u => u.HasPermission(PermissionKind.IsAdministrator))
            .SelectMany(u =>
                StreamyfinPlugin.Instance?.Database.GetUserDeviceTokens(u.Id) ?? Enumerable.Empty<DeviceToken>()) 
        ?? Array.Empty<DeviceToken>()
    ).ToList();

    public static List<string> GetAdminTokens(this IUserManager? manager) => 
        manager?.GetAdminDeviceTokens().Select(deviceToken => deviceToken.Token).ToList() ?? [];
}