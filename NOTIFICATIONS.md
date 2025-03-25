# Streamyfin client notifications

Our plugin can consume any event and forward them to your streamyfin users!

There are currently a few jellyfin events directly supported by our plugin!

Events:
- Item Added (Everyone)
- Session Started (Admin only)
- User Locked out (Admin + user who was locked out)
- Playback started (Admin only)

These can be enabled/disabled inside our plugins page as a setting!


## Custom webhook notifications
If you want to directly start using the notification endpoint with other services, take a look at our examples on how to do so!

Custom webhook examples:
- [Jellyfin](#Jellyfin)
- [Jellyseerr](#Jellyseerr)

---

# Endpoint (Authorization required)

`http(s)://server.instance/Streamyfin/notification`

This endpoint requires two headers:

key: `Content-Type`<br>
value: `application/json`

key: `Authorization`<br>
value: `MediaBrowser Token="{apiKey}"`

**You can generate a jellyfin API Key by going to** 
`Dashboard -> Advanced (bottom left) -> API Keys -> Click on (+) to genreate a key` 

## Template
```json
[
  {
    "title": "string",    // Notification title (required)
    "subtitle": "string", // Notification subtitle (Visible only to iOS users)
    "body": "string",     // Notification body (required)
    "userId": "string",   // Target jellyfin user id this notification is for
    "username": "string", // Target jellyfin username this notification is for
    "isAdmin": false      // Boolean to determine if notification also targets admins.
  }
]
```

## Notifying all users
To do this all you have to do is populate title & body! Other fields are not required!

---

# Examples

## Jellyfin
You can use the [jellyfin-webhook-plugin](https://github.com/jellyfin/jellyfin-plugin-webhook) to create a notification based on any event they offer.

- Visit the webhooks config page
- Click "Add Generic Destination"
- Webhook Url should be the url example from above
- Selected notification type

If we don't directly support an event you'll want to create a separate webhook destination for each event so we can avoid filtering on our end.

**We are currently looking into supporting as many of the jellyfin events so that you don't have to worry about configuring them!**

### examples

- [Item Added](#item-added-notification) 
  - We currently support this on our end with the enhancement of:
    - reducing spam when multiple episodes are added for a season in a short period of time.
    - deep link into item page to start playing item from notification


### Item added notification
- Select event "Item Added"
- Paste in template below

```json
[
    {
        {{#if_equals ItemType 'Movie'}}
          "title": "{{{Name}}} ({{Year}}) added",
          "body": "Watch movie now"
        {{/if_equals}}
        {{#if_equals ItemType 'Season'}}
          "title": "{{{SeriesName}}} season added",
          "body": "Watch season '{{{Name}}}' now"
        {{/if_equals}}
        {{#if_equals ItemType 'Episode'}}
          "title": "{{{SeriesName}}} S{{SeasonNumber00}}E{{EpisodeNumber00}} added",
          "body": "Watch episode '{{{Name}}}' now"
        {{/if_equals}}
    }
]
```

---

## Jellyseerr

You can go to your jellyseerr instances notification settings to forward events

- Go to Settings > Notifications > Webhook
- Check "Enable Agent"
- Enter notification endpoint as "Webhook URL"
- Copy an example below

[Template variable help](https://docs.overseerr.dev/using-overseerr/notifications/webhooks#template-variables)


## Issues notification 

- Copy json below and paste in as JSON Payload
- Select Notification Types 
  - Issue Reported
  - Issue Commented
  - Issue Resolved
  - Issue Reopened

```json
[
  {
    "title": "{{event}}",
    "body": "{{subject}}: {{message}}",
    "isAdmin": true
  },
  {
    "title": "{{event}} - {{subject}}",
    "body": "{{commentedBy_username}}: {{comment_message}}",
    "isAdmin": true
  }
]
```

