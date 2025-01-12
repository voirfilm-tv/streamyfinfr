using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using MediaBrowser.Common.Api;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Jellyfin.Plugin.Streamyfin.Configuration;
using NJsonSchema.Generation;

namespace Jellyfin.Plugin.Streamyfin.Api;

public class JsonStringResult : ContentResult
{
  public JsonStringResult(string json)
  {
    Content = json;
    ContentType = "application/json";
  }
}

public class ConfigYamlRes
{
  public string Value { get; set; } = default!;
}

public class ConfigSaveResponse
{
  public bool Error { get; set; }
  public string Message { get; set; } = default!;
}

//public class ConfigYamlReq {
//  public string? Value { get; set; }
//}

/// <summary>
/// CollectionImportController.
/// </summary>
[ApiController]
[Route("streamyfin")]
public class StreamyfinController : ControllerBase
{
  private readonly ILogger<StreamyfinController> _logger;
  private readonly ILoggerFactory _loggerFactory;
  private readonly IServerConfigurationManager _config;
  private readonly IUserManager _userManager;
  private readonly ILibraryManager _libraryManager;
  private readonly IDtoService _dtoService;

  public StreamyfinController(
      ILoggerFactory loggerFactory,
      IDtoService dtoService,
      IServerConfigurationManager config,
      IUserManager userManager,
      ILibraryManager libraryManager)
  {
    _loggerFactory = loggerFactory;
    _logger = loggerFactory.CreateLogger<StreamyfinController>();
    _dtoService = dtoService;
    _config = config;
    _userManager = userManager;
    _libraryManager = libraryManager;

    _logger.LogInformation("StreamyfinController Loaded");
  }

  [HttpPost("config/yaml")]
  [Authorize(Policy = Policies.RequiresElevation)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public ActionResult<ConfigSaveResponse> saveConfig(
    [FromBody, Required] ConfigYamlRes config
  )
  {
    var deserializer = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
      .Build();

    Config p;
    try
    {
      p = deserializer.Deserialize<Config>(config.Value);
    }
    catch (Exception e)
    {

      return new ConfigSaveResponse { Error = true, Message = e.ToString() };
    }

    var c = StreamyfinPlugin.Instance!.Configuration;
    c.Config = p;
    StreamyfinPlugin.Instance!.UpdateConfiguration(c);

    return new ConfigSaveResponse { Error = false };
  }

  [HttpGet("config")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public ActionResult<Config> getConfig(
  )
  {
    var config = StreamyfinPlugin.Instance!.Configuration.Config;
    return config;
  }

  [HttpGet("config/schema")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public ActionResult getConfigSchema(
  )
  {
    var settings = new SystemTextJsonSchemaGeneratorSettings();
    settings.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    var schema = JsonSchemaGenerator.FromType<Config>(settings);
    return new JsonStringResult(schema.ToJson());
  }

  [HttpGet("config/yaml")]
  [Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public ActionResult<ConfigYamlRes> getConfigYaml(
  )
  {
    var config = StreamyfinPlugin.Instance!.Configuration.Config;
    var serializer = new SerializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)
      .Build();
    var yaml = serializer.Serialize(config);
    return new ConfigYamlRes { Value = yaml };
  }

  //[HttpGet("config.yaml")]
  //[Authorize]
  //[ProducesResponseType(StatusCodes.Status200OK)]
  //public ActionResult<string> getConfigYamlTest(
  // )
  //{
  //  var config = StreamyfinPlugin.Instance!.Configuration.Config;
  // var serializer = new SerializerBuilder()
  //.WithNamingConvention(CamelCaseNamingConvention.Instance)
  //.ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
  //.Build();
  //  var yaml = serializer.Serialize(config);
  //return yaml;


  //}

}
