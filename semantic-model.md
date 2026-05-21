# Semantic Database Model

This project uses SQL Server with Entity Framework Core and the database name `cs2scope-db`.

## Tables and Core Fields

### Users
Base table for shared user fields.
- `Id` - primary key
- `Username`
- `DisplayName`
- `Bio`
- `Email`
- `CountryCode`
- `RegisteredAtUtc`
- `IsSuspended`

### ForumUsers
Forum account details stored in a separate table.
- `Id` - primary key and foreign key to `Users`
- `LastActiveAtUtc`
- `IsPremiumMember`
- `Password`

### AdminUsers
Admin and moderation accounts stored in a separate table.
- `Id` - primary key and foreign key to `Users`
- `HiredAtUtc`
- `LastModerationActionAtUtc`
- `PermissionGroup`

### EventVenues
Arena and venue metadata.
- `Id` - primary key
- `Name`
- `City`
- `CountryCode`
- `Capacity`
- `IsIndoor`
- `SurfaceType`

### Teams
Competitive teams.
- `Id` - primary key
- `Name`
- `Tag`
- `CountryCode`
- `WorldRanking`
- `FoundedYear`
- `PrizeMoneyUsd`
- `LastRosterUpdateUtc`

### Players
Roster entries for teams.
- `Id` - primary key
- `Nickname`
- `FullName`
- `CountryCode`
- `DateOfBirth`
- `Role`
- `Rating2`
- `TotalMapsPlayed`
- `JoinedTeamAtUtc`
- `TeamId` - foreign key to `Teams`

### Matches
Series records tied to events and two teams.
- `Id` - primary key
- `ScheduledAtUtc`
- `IsFinished`
- `Format`
- `TeamAScore`
- `TeamBScore`
- `FinishedAtUtc`
- `EventId` - foreign key to `Tournaments`
- `TeamAId` - foreign key to `Teams`
- `TeamBId` - foreign key to `Teams`

### MatchMaps
Per-map results for finished matches.
- `Id` - primary key
- `MapSequence`
- `Map`
- `TeamAScore`
- `TeamBScore`
- `WentToOvertime`
- `MatchId` - foreign key to `Matches`

### Tournaments
Event or tournament records.
- `Id` - primary key
- `Name`
- `Organizer`
- `Tier`
- `PrizePoolUsd`
- `StartDateUtc`
- `EndDateUtc`
- `IsLan`
- `EventVenueId` - foreign key to `EventVenues`
- `AdminUserId` - foreign key to `Users` / `AdminUsers`

### Forums
Forum threads.
- `Id` - primary key
- `Title`
- `Content`
- `Category`
- `CreatedAtUtc`
- `LastUpdatedAtUtc`
- `ViewCount`
- `IsPinned`
- `IsLocked`
- `AuthorId` - foreign key to `Users` / `ForumUsers`
- `TournamentId` - foreign key to `Tournaments`

### ForumComments
Replies inside forum threads.
- `Id` - primary key
- `ForumId` - foreign key to `Forums`
- `AuthorId` - foreign key to `Users` / `ForumUsers`
- `Content`
- `CreatedAtUtc`
- `IsEdited`

### EventTeams
Join table for the many-to-many relation between tournaments and teams.
- `TeamsId` - foreign key to `Teams`
- `TournamentsId` - foreign key to `Tournaments`

### ForumUserFavoriteTeams
Join table for saved team favorites.
- `ForumUserId` - foreign key to `Users`
- `TeamId` - foreign key to `Teams`

### ForumUserFavoritePlayers
Join table for saved player favorites.
- `ForumUserId` - foreign key to `Users`
- `PlayerId` - foreign key to `Players`

## Relationships

- One `Team` has many `Players`.
- One `EventVenue` has many `Tournaments`.
- One `AdminUser` can manage many `Tournaments`.
- One `Tournament` can include many `Teams`, and one `Team` can appear in many `Tournaments`.
- One `Tournament` can have many `ForumThreads`.
- One `ForumUser` can author many `Forums` and many `ForumComments`.
- One `ForumUser` can save many favorite `Teams` and many favorite `Players`.
- One `Forum` can have many `ForumComments`.
- One `Forum` can optionally point to one `Tournament`.
- One `Tournament` can have many `Matches`.
- One `Match` belongs to one `Tournament` and two `Teams`.
- One `Match` can have many `MatchMaps`.

## Notes

- `User` inheritance is stored with TPT mapping across `Users`, `ForumUsers`, and `AdminUsers`.
- Decimal values such as prize money and ratings use explicit SQL Server precision.
- Enum properties like `Tier`, `Role`, and `Category` are stored as integers.
- Match format and map pool enums are also stored as integers.
