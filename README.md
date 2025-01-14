## Companion plugin for [Streamyfin](https://github.com/fredrikburmester/streamyfin)

> A plugin for Jellyfin that allows a centralised configuration of Streamyfin.

### Required minimum versions
- Streamyfin App version: **0.25.0** (currently in Beta) 
- Plugin version: **0.33.0.0**

### What is this?

The Jellyfin Plugin for Streamyfin is a plugin you install into Jellyfin that hold all settings for the client Streamyfin. This allows you to syncronize settings accross all your users, like: 

- Auto log in to Jellyseerr without the user having to do anythin
- Choose the default languages 
- Set download method and search provider
- Customize homescreen
- And more...

### Customize home screen

It's possible to define a custom homescreen with this plugin.
See the home definition in the example below for more info.

Together with the [collection import](https://github.com/lostb1t/jellyfin-plugin-collection-import) plugin, one can make very dynamic homescreens.


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

  # Home
  # home
  home:
    locked: true
    value:
      sections:
        Recent movies:
          orientation: horizontal
          items:
              includeItemTypes:
                - Movie
        Recent shows:
          items:
            includeItemTypes:
            - Series
        Continue Watching:
          items:
            filters:
              - IsResumable
            includeItemTypes:
            - Movie
            - Series
        My collection:
          items:
            # parentID is the collection id. You can find it in the url on the web collection page
            parentId: ab7b2bcacafa97a2ad7d72d86eb1b408
            includeItemTypes:
            - Movie
            - Series

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

### Install Process

1. In jellyfin, go to dashboard -> plugins -> Repositories -> add and paste this link: https://raw.githubusercontent.com/streamyfin/jellyfin-plugin-streamyfin/main/manifest.json
2. Go to Catalog and search for Streamyfin
3. Click on it and install
4. Restart Jellyfin

### Create release

- bump version in makefile
- run `make release`
- commit and push changes made release script
