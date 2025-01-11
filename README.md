## Companion app for [Streamyfin](https://github.com/fredrikburmester/streamyfin)

> Allows centralised configuration of Streamyfin.

### What is this?

The Jellyfin Plugin for Streamyfin is a plugin you install into Jellyfin that hold all settings for the client Streamyfin. This allows you to syncronize settings accross all your users, like: 

- Auto log in to Jellyseerr without the user having to do anythin
- Choose the default languages 
- Set download method and search provider
- And more...

### Config example

```yaml
# You can remove any settings you do not need configured.

# Format Example
# settingName:
#   locked: true | false # if true, locks the setting from modification in app. Default false.
#   value: value # Value you want the setting to be. Editor will give you type suggestion for a specific setting.

# Example below shows all supported settings at this time.
settings:
  # Media Controls
  forwardSkipTime:
  rewindSkipTime: 

  # Audio Controls
  rememberAudioSelections:
  
  # Subtitles
  subtitleMode:
  rememberSubtitleSelections:
  subtitleSize:
  
  # Other
  autoRotate:
  defaultVideoOrientation:
  safeAreaInControlsEnabled:
  showCustomMenuLinks:
  hiddenLibraries:
  disableHapticFeedback:
  
  # Downloads
  downloadMethod:
  remuxConcurrentLimit:
  autoDownload:
  optimizedVersionsServerUrl:

  # Jellyseerr 
  jellyseerrServerUrl:
  
  # Search
  searchEngine:
  marlinServerUrl:

  # Popular Lists
  usePopularPlugin:
  mediaListCollectionIds:

  # Misc.
  libraryOptions:
    locked: false
    value:
      display: list | row
      cardStyle: detailed | compact
      imageStyle: cover | poster
      showTitles: boolean
      showStats: boolean
```

#### Supported Streamyfin App conxfigurations
[Settings.cs](Jellyfin.Plugin.Streamyfin/Configuration/Settings/Settings.cs)

repo url: https://raw.githubusercontent.com/streamyfin/jellyfin-plugin-streamyfin/main/manifest.json

### Create release

- bump version in makefile
- run `make release`
- commit and push changes made release script
