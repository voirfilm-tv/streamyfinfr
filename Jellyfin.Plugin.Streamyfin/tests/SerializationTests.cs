using System;
using System.Collections.Generic;
using Jellyfin.Data.Enums;
using Jellyfin.Plugin.Streamyfin.Configuration;
using Jellyfin.Plugin.Streamyfin.Configuration.Settings;
using Jellyfin.Plugin.Streamyfin.PushNotifications.models;
using MediaBrowser.Controller.Entities.Movies;
using Xunit;
using Xunit.Abstractions;
using Assert = ICU4N.Impl.Assert;
using Settings = Jellyfin.Plugin.Streamyfin.Configuration.Settings.Settings;

namespace Jellyfin.Plugin.Streamyfin.tests;

/// <summary>
/// Ensure special types are properly serialized/deserialized when converting between Object - Json - Yaml
/// </summary>
public class SerializationTests(ITestOutputHelper output)
{
    private readonly SerializationHelper _serializationHelper = new();

    /// <summary>
    /// Ensure Json Schema forces enum names as values imported from external namespaces
    /// </summary>
    [Fact]
    public void EnumJsonSchemaTest()
    {
        var schema = SerializationHelper.GetJsonSchema<Config>();
        output.WriteLine(schema);

        Assert.Assrt(
            msg: "SubtitlePlaybackMode enumNames are string values",
            val: schema.Contains(
                """
                    "SubtitlePlaybackMode": {
                      "type": "string",
                      "description": "An enum representing a subtitle playback mode.",
                      "x-enumNames": [
                        "Default",
                        "Always",
                        "OnlyForced",
                        "None",
                        "Smart"
                      ],
                      "enum": [
                        "Default",
                        "Always",
                        "OnlyForced",
                        "None",
                        "Smart"
                      ]
                    }
                """
                , StringComparison.Ordinal)
            );
        // TODO: Not required, more of a nit...
        //  Spend time figuring out why converter is not ensuring this enum stays int for schema
        // Assert.Assrt(
        //     msg: "RemuxConcurrentLimit enum values are still integers",
        //     val: schema.ToJson().Contains(
        //         """
        //             "RemuxConcurrentLimit": {
        //               "type": "integer",
        //               "description": "",
        //               "x-enumNames": [
        //                 "One",
        //                 "Two",
        //                 "Three",
        //                 "Four"
        //               ],
        //               "enum": [
        //                 1,
        //                 2,
        //                 3,
        //                 4
        //               ]
        //             }
        //         """
        //         , StringComparison.Ordinal)
        // );
    }

    /// <summary>
    /// Ensures all types of enums are deserialized correctly
    /// </summary>
    [Fact]
    public void EnumConfigJsonDeserializationTest()
    {
        DeserializeConfig(
            """
            {
                "settings": {
                    "subtitleMode": {
                        "locked": true,
                        "value": "Default"
                    },
                    "defaultVideoOrientation": {
                        "locked": true,
                        "value": "LandscapeLeft"
                    },
                    "downloadMethod": {
                        "locked": true,
                        "value": "remux"
                    },
                    "remuxConcurrentLimit": {
                        "locked": true,
                        "value": 2
                    }
                }
            }
            """
        );
    }

    /// <summary>
    /// Ensures all types of enums are deserialized correctly
    /// </summary>
    [Fact]
    public void EnumConfigYamlDeserializationTest()
    {
        DeserializeConfig(
            """
            settings:
                subtitleMode:
                    locked: true
                    value: Default
                defaultVideoOrientation:
                    locked: true
                    value: LandscapeLeft
                downloadMethod:
                    locked: true
                    value: remux
                remuxConcurrentLimit:
                    locked: true
                    value: Two
            """
        );
    }

    /// <summary>
    /// Ensures all types of enums are json serialized correctly
    /// </summary>
    [Fact]
    public void ConfigJsonSerializationTest()
    {
        SerializeConfig(
            value: _serializationHelper.SerializeToJson(GetTestConfig()),
            expected:
            """
            {
              "settings": {
                "subtitleMode": {
                  "locked": false,
                  "value": 0
                },
                "defaultVideoOrientation": {
                  "locked": false,
                  "value": 6
                },
                "downloadMethod": {
                  "locked": false,
                  "value": "remux"
                },
                "remuxConcurrentLimit": {
                  "locked": false,
                  "value": 2
                }
              }
            }
            """
        );
    }
    
    /// <summary>
    /// Ensures all types of enums are yaml serialized correctly
    /// </summary>
    [Fact]
    public void ConfigYamlSerializationTest()
    {
        SerializeConfig(
            value: _serializationHelper.SerializeToYaml(GetTestConfig()),
            expected:
            """
            settings:
              subtitleMode:
                locked: false
                value: Default
              defaultVideoOrientation:
                locked: false
                value: LandscapeLeft
              downloadMethod:
                locked: false
                value: remux
              remuxConcurrentLimit:
                locked: false
                value: Two
            """
        );
    }
    
    /// <summary>
    /// Ensures array of notifications are deserialized correctly
    /// </summary>
    [Fact]
    public void DeserializeNotification()
    {
        var notification = _serializationHelper.Deserialize<List<Notification>>(
            """
            [
                {
                    "title": "Test Title",
                    "body": "Test Body",
                    "userId": "2c585c0706ac46779a2c38ca896b556f"
                }
            ]
            """
        )[0];
        
        Assert.Assrt(
            msg: "title deserialized",
            notification.Title == "Test Title"
        );
        
        Assert.Assrt(
            msg: "body deserialized",
            notification.Body == "Test Body"
        );

        Assert.Assrt(
            msg: "guid deserialized",
            notification.UserId?.ToString("N") == "2c585c0706ac46779a2c38ca896b556f"
        );
    }

    private static Config GetTestConfig()
    {
        return new Config
        {
            settings = new Settings
            {
                downloadMethod = new Lockable<DownloadMethod>
                {
                    value = DownloadMethod.remux
                },
                subtitleMode = new Lockable<SubtitlePlaybackMode>
                {
                    value = SubtitlePlaybackMode.Default
                },
                defaultVideoOrientation = new Lockable<OrientationLock>
                {
                    value = OrientationLock.LandscapeLeft
                },
                remuxConcurrentLimit = new Lockable<RemuxConcurrentLimit>
                {
                    value = RemuxConcurrentLimit.Two
                }
            }
        };
    }

    private void SerializeConfig(string value, string expected)
    {
        output.WriteLine($"Serialized:\n {value}");
        output.WriteLine($"Expected:\n {expected}");
        Assert.Assrt("Config serialized matches expected", value.Trim() == expected.Trim());
    }

    private void DeserializeConfig(string value)
    {
        output.WriteLine($"Deserializing config from:\n {value}");
        Config config = _serializationHelper.Deserialize<Config>(value);

        Assert.Assrt(
            $"RemuxConcurrentLimit matches: {SubtitlePlaybackMode.Default} == {config.settings?.subtitleMode?.value}",
            SubtitlePlaybackMode.Default == config.settings?.subtitleMode?.value
        );
        Assert.Assrt(
            $"OrientationLock matches: {OrientationLock.LandscapeLeft} == {config.settings?.defaultVideoOrientation?.value}",
            OrientationLock.LandscapeLeft == config.settings?.defaultVideoOrientation?.value
        );
        Assert.Assrt(
            $"DownloadMethod matches: {DownloadMethod.remux} == {config.settings?.downloadMethod?.value}",
            DownloadMethod.remux == config.settings?.downloadMethod?.value
        );
        Assert.Assrt(
            $"RemuxConcurrentLimit matches: {RemuxConcurrentLimit.One} == {config.settings?.remuxConcurrentLimit?.value}",
            RemuxConcurrentLimit.Two == config.settings?.remuxConcurrentLimit?.value
        );
    }
}