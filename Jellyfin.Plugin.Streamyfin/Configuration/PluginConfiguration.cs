#pragma warning disable CA2227
#pragma warning disable CS0219

using System;
using System.Collections.Generic;
using MediaBrowser.Model.Plugins;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Jellyfin.Plugin.Streamyfin.Configuration;


/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
  //public string Yaml { get; set; }
  public Config Config { get; set; }

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

    var deserializer = new DeserializerBuilder()
    //.ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitEmptyCollections)
    .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
    .Build();


    Config = deserializer.Deserialize<Config>(Yaml);

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
