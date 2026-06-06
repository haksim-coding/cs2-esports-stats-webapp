    # API Attributes Reference

This document lists the attributes used by the API layer for teams, players, events, and matches.

## Controller Attributes

| Attribute | Where it is used | Purpose |
| --- | --- | --- |
| `Route` | `Controllers/Api/*Controller.cs` | Sets the API prefix, for example `api/teams`, `api/players`, `api/events`, and `api/matches`. |
| `ApiController` | `Controllers/Api/*Controller.cs` | Enables API-specific behavior such as automatic model validation responses and binding conventions. |

## Action Attributes

| Attribute | Where it is used | Purpose |
| --- | --- | --- |
| `HttpGet` | List and details endpoints | Marks a method as a GET endpoint. Used for collection reads and single-item reads. |
| `HttpPost` | Create endpoints | Marks a method as a POST endpoint for creating a new record. |
| `HttpPut` | Update endpoints | Marks a method as a PUT endpoint for updating an existing record. |
| `HttpDelete` | Delete endpoints | Marks a method as a DELETE endpoint for removing a record. |
| `FromQuery` | Query parameters on GET endpoints | Binds values from the query string, such as search filters. |
| `FromBody` | POST and PUT request bodies | Binds JSON request bodies to DTOs. |

## Validation Attributes On DTOs

| Attribute | Where it is used | Purpose |
| --- | --- | --- |
| `Required` | Upsert DTOs | Forces the client to send a value for the property. |
| `StringLength` | Upsert DTOs | Limits string length and enforces minimum length where needed. |
| `Range` | Upsert DTOs | Validates numeric values such as rankings, prize money, dates-as-numbers, and IDs. |
| `MaxLength` | Team upsert DTO | Limits the number of selected player IDs to five. |
| `EnumDataType` | Player, event, and match upsert DTOs | Validates enum-backed fields such as role, tier, and match format. |

## Used In The Current API Layer

### Teams

- `Route("api/teams")`
- `ApiController`
- `HttpGet`
- `HttpGet("{id:int}")`
- `HttpPost`
- `HttpPut("{id:int}")`
- `HttpDelete("{id:int}")`
- `FromQuery`
- `FromBody`
- `Required`
- `StringLength`
- `Range`
- `MaxLength`

### Players

- `Route("api/players")`
- `ApiController`
- `HttpGet`
- `HttpGet("{id:int}")`
- `HttpPost`
- `HttpPut("{id:int}")`
- `HttpDelete("{id:int}")`
- `FromQuery`
- `FromBody`
- `Required`
- `StringLength`
- `Range`
- `EnumDataType`

### Events

- `Route("api/events")`
- `ApiController`
- `HttpGet`
- `HttpGet("{id:int}")`
- `HttpPost`
- `HttpPut("{id:int}")`
- `HttpDelete("{id:int}")`
- `FromQuery`
- `FromBody`
- `Required`
- `StringLength`
- `Range`
- `EnumDataType`

### Matches

- `Route("api/matches")`
- `ApiController`
- `HttpGet`
- `HttpGet("{id:int}")`
- `HttpPost`
- `HttpPut("{id:int}")`
- `HttpDelete("{id:int}")`
- `FromQuery`
- `FromBody`
- `Required`
- `Range`
- `EnumDataType`

## Notes

- The `{id:int}` route constraint is not an attribute, but it is part of the endpoint route template and is used on the item-level GET, PUT, and DELETE methods.
- The API layer currently does not use authorization attributes yet. That will be added later with Identity.