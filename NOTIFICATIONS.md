# Streamyfin client notifications

Our plugin can consume any event and forward them to your streamyfin users!

Examples:
- [Jellyfin](#Jellyfin)
- [Jellyseerr](#Jellyseerr)

## Endpoint (Authorization required)

`http(s)://server.instance/Streamyfin/notification`

This endpoint requires two headers:

key: `Content-Type`<br>
value: `application/json`

key: `Authorization`<br>
value: `MediaBrowser Token="{apiKey}"`

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

---

# Examples

## Jellyfin
You can use the [jellyfin-webhook-plugin](https://github.com/jellyfin/jellyfin-plugin-webhook) to create a notification based on any event they offer.

- Visit the webhooks config page
- Click "Add Generic Destination"
- Webhook Url should be the url example from above
- Selected notification type

You'll want to create a separate webhook destination for each event so we can avoid filtering on our end.

### examples

- [Sessions Starting](#admin-notification-when-a-session-starts)
- [Item Added](#item-added-notification)
- [User locked out](#user-locked-out)

**We are currently looking into using these notifications directly without us having to forward these to ourselves** 

### (Admin) notification when a session starts
Admin is in the title since you probably don't want all your users getting this notification
- Select event "Session Start"
- Paste in template below

```json
[
    {
        "title": "Session started",
        "body": "{{{NotificationUsername}}} now watching",
        "isAdmin": true
    }
]
```

### Item added notification
- Select event "Item Added"
- Paste in template below

```json
[
    {
        {{#if_equals ItemType 'Movie'}}
          "title": "{{{Name}}} ({{Year}}) added",
          "body": "Watch movie now"
        {{else}}
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


### User locked out
- Select event "User Locked Out"
- Paste in template below

```json
[
  {
    "title": "{{{User}}} is now locked out",
    "body": "Contact admin for reset",
    "userId": "{{{UserId}}}",
    "isAdmin": true
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

