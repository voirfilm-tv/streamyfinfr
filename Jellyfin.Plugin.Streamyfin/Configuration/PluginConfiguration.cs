#pragma warning disable CA2227
#pragma warning disable CS0219

using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Streamyfin.Configuration;


/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
  //public string Yaml { get; set; }
  public Config Config { get; set; }
  private readonly SerializationHelper _serializationHelper;

  public PluginConfiguration(
    SerializationHelper serializationHelper
  )
  {
    _serializationHelper = serializationHelper;
  }
  

  public PluginConfiguration()
  {
    var Yaml = @"
# You can remove any settings you do not need configured.

# Format Example
# settingName:
#   locked: true | false # if true, locks the setting from modification in app. Default false.
#   value: value # Value you want the setting to be. Editor will give you type suggestion for a specific setting.

# Example below shows all supported settings at this time.
settings:
  downloadMethod:
    locked: true
    value: REMUX

  # # Media Controls
  # forwardSkipTime:
  # rewindSkipTime: 

  # # Audio Controls
  # rememberAudioSelections:
  
  # # Subtitles
  # subtitleMode:
  # rememberSubtitleSelections:
  # subtitleSize:
  
  # # Other
  # autoRotate:
  # defaultVideoOrientation:
  # safeAreaInControlsEnabled:
  # showCustomMenuLinks:
  # hiddenLibraries:
  # disableHapticFeedback:
  
  # # Downloads
  # downloadMethod:
  # remuxConcurrentLimit:
  # autoDownload:
  # optimizedVersionsServerUrl:

  # # Jellyseerr 
  # jellyseerrServerUrl:
  
  # # Search
  # searchEngine:
  # marlinServerUrl:

  # # Popular Lists
  # usePopularPlugin:
  # mediaListCollectionIds:

  # # Misc.
  # libraryOptions:
";

    Config = _serializationHelper?.Deserialize<Config>(Yaml);

    /*
    Config = new Config{
      marlinSearch = new Search{
        enabled = false,
        url = ""
      },
      home = new Home{
        sections = new SerializableDictionary<string, Section>
      {
         { "Trending collection", new Section{
            style = SectionStyle.portrait,
            type = SectionType.carousel,
            items = new SectionItemResolver 
              {args = new ItemArgs{
              parentId = "YOURCOLLECTIONID"
            } } } ,
          { "Continue Watching", new Section{
            style = SectionStyle.portrait,
            type = SectionType.carousel,
            items = new SectionItemResolver 
              {args = new ItemArgs{
              filters = "YOURCOLLECTIONID"
            } } },
          { "Anime", new Section{
            style = SectionStyle.portrait,
            type = SectionType.row,
            items = new SectionItemResolver{ args = new ItemArgs{
             genres = new List<string>{"Anime"}
            }
          } } } }
      }
      }

    };
    */
    //Yaml
    /* 
  SfConfig = "test";
  Yaml = @"home:
sections:
  Trending:
    style: portrait
    type: row 
    source: 
      resolver: items
      args: 
        sortBy: AddedDate
        sortOrder: Ascending
        filterByGenre: [""Anime"", ""Comics""]";
        */
  }
}
