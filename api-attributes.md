# API Request Reference

Use JSON property names exactly as shown below. Send JSON requests with:

```http
Content-Type: application/json
```

- Dates use ISO 8601, preferably UTC: `"2026-07-10T18:00:00Z"`.
- Enum values are sent as **numbers**, not names. See the enum tables at the bottom.
- `GET` and `DELETE` requests do not have a JSON body.
- `POST`, `PUT`, and `DELETE` require an authenticated admin. Team, player, and match changes require `SuperAdmin`. Event changes require an admin allowed to manage that organizer.
- `PUT` replaces the editable data, so send the complete JSON object rather than only changed fields.

## Endpoint Summary

| Method | Endpoint | JSON body |
| --- | --- | --- |
| `GET` | `/api/teams` | None |
| `GET` | `/api/teams/{id}` | None |
| `POST` | `/api/teams` | Team JSON |
| `PUT` | `/api/teams/{id}` | Team JSON |
| `DELETE` | `/api/teams/{id}` | None |
| `GET` | `/api/players` | None |
| `GET` | `/api/players/{id}` | None |
| `POST` | `/api/players` | Player JSON |
| `PUT` | `/api/players/{id}` | Player JSON |
| `DELETE` | `/api/players/{id}` | None |
| `GET` | `/api/events` | None |
| `GET` | `/api/events/{id}` | None |
| `POST` | `/api/events` | Event JSON |
| `PUT` | `/api/events/{id}` | Event JSON |
| `DELETE` | `/api/events/{id}` | None |
| `GET` | `/api/matches` | None |
| `GET` | `/api/matches/{id}` | None |
| `POST` | `/api/matches` | Match JSON |
| `PUT` | `/api/matches/{id}` | Match JSON |
| `DELETE` | `/api/matches/{id}` | None |

`/api/team` is also accepted as an alias for every `/api/teams` endpoint.

## GET Query Parameters

| Endpoint | Optional query parameters |
| --- | --- |
| `GET /api/teams` | `query`: searches team name or tag |
| `GET /api/players` | `query`: searches nickname, full name, or country code; must contain at least 2 characters. `currentTeamId`: with `query`, limits results to free agents and members of that team |
| `GET /api/events` | `query`: searches event name or organizer; must contain at least 2 characters |
| `GET /api/matches` | `query`: searches event and team names or tags; must contain at least 2 characters |

Example:

```http
GET /api/players?query=donk&currentTeamId=2
```

## Teams

Use the same body for:

- `POST /api/teams`
- `PUT /api/teams/{id}`

```json
{
  "name": "Team Spirit",
  "tag": "Spirit",
  "countryCode": "RU",
  "foundedYear": 2015,
  "prizeMoneyUsd": 5000000,
  "selectedPlayerIds": [1, 2, 3, 4, 5]
}
```

| Property | Rules |
| --- | --- |
| `name` | Required string, 2-80 characters, must be unique |
| `tag` | Required string, 2-10 characters, must be unique |
| `countryCode` | Required 2-character string |
| `foundedYear` | Required integer from 1990 to 2100 |
| `prizeMoneyUsd` | Required decimal number |
| `selectedPlayerIds` | Optional array of up to 5 unique player IDs; players must be free agents or already on this team |

Do not send `id`, `worldRanking`, or `lastRosterUpdateUtc`; the server manages them.

## Players

Use the same body for:

- `POST /api/players`
- `PUT /api/players/{id}`

```json
{
  "nickname": "donk",
  "fullName": "Danil Kryshkovets",
  "countryCode": "RU",
  "dateOfBirth": "2007-01-25T00:00:00Z",
  "role": 1,
  "rating2": 1.35,
  "totalMapsPlayed": 250,
  "teamId": 1
}
```

| Property | Rules |
| --- | --- |
| `nickname` | Required string, 2-40 characters, must be unique |
| `fullName` | Required string, 2-80 characters |
| `countryCode` | Required 2-character string |
| `dateOfBirth` | Required ISO 8601 date/time |
| `role` | Required `PlayerRole` number |
| `rating2` | Required decimal from 0 to 5 |
| `totalMapsPlayed` | Required integer, 0 or greater |
| `teamId` | Existing team ID, or `null` for a free agent |

Do not send `id`, `imagePath`, or `joinedTeamAtUtc`; the server manages them.

## Events

Use the same body for:

- `POST /api/events`
- `PUT /api/events/{id}`

```json
{
  "name": "IEM Cologne 2026",
  "organizer": "ESL",
  "tier": 1,
  "prizePoolUsd": 1000000,
  "startDateUtc": "2026-07-23T00:00:00Z",
  "endDateUtc": "2026-08-02T00:00:00Z",
  "isLan": true,
  "bannerImagePath": "/images/events/banners/iem-cologne-2026.png",
  "eventVenueId": 1,
  "selectedTeamIds": [1, 2, 3, 4]
}
```

| Property | Rules |
| --- | --- |
| `name` | Required string, 2-120 characters, must be unique |
| `organizer` | Required string, 2-120 characters; the authenticated admin must be allowed to manage it |
| `tier` | Required `EventTier` number |
| `prizePoolUsd` | Required decimal from 0 to 1,000,000,000 |
| `startDateUtc` | Required ISO 8601 date/time |
| `endDateUtc` | Required ISO 8601 date/time, on or after `startDateUtc` |
| `isLan` | Required boolean |
| `bannerImagePath` | Optional string up to 260 characters; accepted paths must begin with `/images/events/banners/` |
| `eventVenueId` | Required existing venue ID, 1 or greater |
| `selectedTeamIds` | Optional array of existing team IDs |

## Matches

Use the same body for:

- `POST /api/matches`
- `PUT /api/matches/{id}`

```json
{
  "scheduledAtUtc": "2026-07-25T18:00:00Z",
  "isFinished": true,
  "format": 3,
  "finishedAtUtc": "2026-07-25T20:30:00Z",
  "eventId": 1,
  "teamAId": 1,
  "teamBId": 2,
  "maps": [
    {
      "mapSequence": 1,
      "map": 2,
      "teamAScore": 13,
      "teamBScore": 8,
      "wentToOvertime": false
    },
    {
      "mapSequence": 2,
      "map": 5,
      "teamAScore": 16,
      "teamBScore": 14,
      "wentToOvertime": true
    }
  ]
}
```

| Property | Rules |
| --- | --- |
| `scheduledAtUtc` | Required ISO 8601 date/time |
| `isFinished` | Boolean; when `true`, `finishedAtUtc` is required |
| `format` | Required `MatchFormat` number |
| `finishedAtUtc` | Use `null` for unfinished matches; otherwise must be on or after `scheduledAtUtc` |
| `eventId` | Required existing event ID |
| `teamAId` | Required existing team ID |
| `teamBId` | Required existing team ID and must differ from `teamAId` |
| `maps` | Required map result array; map sequence numbers must be unique and start at 1 |

Each item in `maps` accepts:

| Property | Rules |
| --- | --- |
| `mapSequence` | Integer starting at 1 |
| `map` | `MapPool` number |
| `teamAScore` | Integer score for Team A |
| `teamBScore` | Integer score for Team B |
| `wentToOvertime` | Boolean |

Do not send map `id` when creating or updating a match.

A finished match must have a winner with exactly the required number of map wins:

| Format | Minimum map results required | Map wins required to finish |
| --- | ---: | ---: |
| Best of 1 | 1 | 1 |
| Best of 3 | 2 | 2 |
| Best of 5 | 3 | 3 |

## Enum Values

### PlayerRole

| Number | Meaning |
| ---: | --- |
| `1` | Rifler |
| `2` | Awper |
| `3` | InGameLeader |
| `4` | EntryFragger |
| `5` | Support |
| `6` | Coach |

### EventTier

| Number | Meaning |
| ---: | --- |
| `0` | Major |
| `1` | S |
| `2` | A |
| `3` | B |
| `4` | C |

### MatchFormat

| Number | Meaning |
| ---: | --- |
| `1` | BestOf1 |
| `3` | BestOf3 |
| `5` | BestOf5 |

### MapPool

| Number | Meaning |
| ---: | --- |
| `1` | Ancient |
| `2` | Mirage |
| `3` | Inferno |
| `4` | Anubis |
| `5` | Nuke |
| `6` | Dust2 |
| `7` | Cache |
