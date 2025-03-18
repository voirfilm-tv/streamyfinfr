## Companion plugin for [Streamyfin](https://github.com/fredrikburmester/streamyfin)

> A plugin for Jellyfin that allows a centralised configuration of Streamyfin.

### What is this?

The Jellyfin Plugin for Streamyfin is a plugin you install into Jellyfin that hold all settings for the client Streamyfin. This allows you to syncronize settings accross all your users, like: 

- Auto log in to Jellyseerr without the user having to do anythin
- Choose the default languages 
- Set download method and search provider
- Customize homescreen
- [Notifications](NOTIFICATIONS.md)
- And more...

### Customize home screen

It's possible to define a custom homescreen with this plugin.
See the home definition in the example below for more info.

Together with the [collection import](https://github.com/lostb1t/jellyfin-plugin-collection-import) plugin, one can make very dynamic homescreens.
Think Trending, popular etc like netflix.

![example](./home.jpg)

### Config examples

See [examples](https://github.com/streamyfin/jellyfin-plugin-streamyfin/tree/main/examples)

### Install Process

1. In jellyfin, go to dashboard -> plugins -> Repositories -> add and paste this link: https://raw.githubusercontent.com/streamyfin/jellyfin-plugin-streamyfin/main/manifest.json
2. Go to Catalog and search for Streamyfin
3. Click on it and install
4. Restart Jellyfin