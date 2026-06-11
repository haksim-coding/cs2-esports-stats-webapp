using cs2_esports.Dtos.Teams;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers.Api;

[Route("api/teams")]
[Route("api/team")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPlayerRepository _playerRepository;

    public TeamsController(ITeamRepository teamRepository, IPlayerRepository playerRepository)
    {
        _teamRepository = teamRepository;
        _playerRepository = playerRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TeamListItemDto>> GetAll([FromQuery] string? query = null)
    {
        var teams = _teamRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            teams = teams
                .Where(team =>
                    team.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                    team.Tag.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Ok(teams.Select(MapToListItemDto));
    }

    [HttpGet("{id:int}")]
    public ActionResult<TeamDetailsDto> GetById(int id)
    {
        var team = _teamRepository.GetById(id);
        if (team is null)
        {
            return NotFound();
        }

        return Ok(MapToDetailsDto(team));
    }

    [HttpPost]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public ActionResult<TeamDetailsDto> Create([FromBody] TeamUpsertDto model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        var normalizedPlayerIds = NormalizePlayerIds(model.SelectedPlayerIds);
        ValidateTeam(model);
        ValidatePlayers(normalizedPlayerIds, null);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var selectedPlayers = _playerRepository.GetByIds(normalizedPlayerIds);
        var team = CreateTeam(model, selectedPlayers);
        _teamRepository.Add(team);

        return CreatedAtAction(nameof(GetById), new { id = team.Id }, MapToDetailsDto(team));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public ActionResult<TeamDetailsDto> Update(int id, [FromBody] TeamUpsertDto model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        var existingTeam = _teamRepository.GetById(id);
        if (existingTeam is null)
        {
            return NotFound();
        }

        var normalizedPlayerIds = NormalizePlayerIds(model.SelectedPlayerIds);
        ValidateTeam(model, id);
        ValidatePlayers(normalizedPlayerIds, id);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var selectedPlayers = _playerRepository.GetByIds(normalizedPlayerIds);
        ApplyModel(existingTeam, model, selectedPlayers);
        _teamRepository.Update(existingTeam);

        return Ok(MapToDetailsDto(existingTeam));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Delete(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        var team = _teamRepository.GetById(id);
        if (team is null)
        {
            return NotFound();
        }

        if ((team.Players?.Any() ?? false) || (team.Tournaments?.Any() ?? false) || (team.HomeMatches?.Any() ?? false) || (team.AwayMatches?.Any() ?? false))
        {
            return Conflict(new { message = "This team cannot be deleted because it is still used by players, events, or matches." });
        }

        _teamRepository.Delete(id);
        return NoContent();
    }

    private void ValidateTeam(TeamUpsertDto model, int? currentTeamId = null)
    {
        var normalizedName = model.Name.Trim();
        var normalizedTag = model.Tag.Trim();
        var teams = _teamRepository.GetAll();

        if (teams.Any(team => team.Id != currentTeamId && team.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Name), "A team with this name already exists.");
        }

        if (teams.Any(team => team.Id != currentTeamId && team.Tag.Equals(normalizedTag, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Tag), "A team with this tag already exists.");
        }
    }

    private void ValidatePlayers(int[] playerIds, int? currentTeamId)
    {
        if (playerIds.Length == 0)
        {
            return;
        }

        if (playerIds.Length > 5)
        {
            ModelState.AddModelError(nameof(TeamUpsertDto.SelectedPlayerIds), "You can select up to 5 players.");
            return;
        }

        var selectedPlayers = _playerRepository.GetByIds(playerIds);
        if (selectedPlayers.Count != playerIds.Length)
        {
            ModelState.AddModelError(nameof(TeamUpsertDto.SelectedPlayerIds), "One or more selected players could not be found.");
            return;
        }

        if (selectedPlayers.Any(player => player.TeamId.HasValue && player.TeamId != currentTeamId))
        {
            ModelState.AddModelError(nameof(TeamUpsertDto.SelectedPlayerIds), "Selected players must be free agents or already belong to this team.");
        }
    }

    private static int[] NormalizePlayerIds(IEnumerable<int> playerIds)
    {
        return playerIds
            .Where(playerId => playerId > 0)
            .Distinct()
            .ToArray();
    }

    private int GetNextWorldRanking()
    {
        return _teamRepository.GetAll().Select(team => team.WorldRanking).DefaultIfEmpty(0).Max() + 1;
    }

    private Team CreateTeam(TeamUpsertDto model, IReadOnlyCollection<Player> selectedPlayers)
    {
        return new Team
        {
            Name = model.Name.Trim(),
            Tag = model.Tag.Trim().ToUpperInvariant(),
            CountryCode = model.CountryCode.Trim().ToUpperInvariant(),
            WorldRanking = GetNextWorldRanking(),
            FoundedYear = model.FoundedYear,
            PrizeMoneyUsd = model.PrizeMoneyUsd,
            LastRosterUpdateUtc = DateTime.UtcNow,
            Players = selectedPlayers.ToList()
        };
    }

    private static void ApplyModel(Team team, TeamUpsertDto model, IReadOnlyCollection<Player> selectedPlayers)
    {
        team.Name = model.Name.Trim();
        team.Tag = model.Tag.Trim().ToUpperInvariant();
        team.CountryCode = model.CountryCode.Trim().ToUpperInvariant();
        team.FoundedYear = model.FoundedYear;
        team.PrizeMoneyUsd = model.PrizeMoneyUsd;
        team.LastRosterUpdateUtc = DateTime.UtcNow;
        team.Players = selectedPlayers.ToList();
    }

    private static TeamListItemDto MapToListItemDto(Team team)
    {
        return new TeamListItemDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag,
            CountryCode = team.CountryCode,
            WorldRanking = team.WorldRanking,
            FoundedYear = team.FoundedYear,
            PrizeMoneyUsd = team.PrizeMoneyUsd,
            LastRosterUpdateUtc = team.LastRosterUpdateUtc,
            PlayerCount = team.Players.Count,
            Players = team.Players
                .OrderBy(player => player.Role)
                .ThenBy(player => player.Nickname)
                .Select(MapToPlayerDto)
                .ToList()
        };
    }

    private static TeamDetailsDto MapToDetailsDto(Team team)
    {
        return new TeamDetailsDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag,
            CountryCode = team.CountryCode,
            WorldRanking = team.WorldRanking,
            FoundedYear = team.FoundedYear,
            PrizeMoneyUsd = team.PrizeMoneyUsd,
            LastRosterUpdateUtc = team.LastRosterUpdateUtc,
            PlayerCount = team.Players.Count,
            Players = team.Players
                .OrderBy(player => player.Role)
                .ThenBy(player => player.Nickname)
                .Select(MapToPlayerDto)
                .ToList()
        };
    }

    private static TeamPlayerDto MapToPlayerDto(Player player)
    {
        return new TeamPlayerDto
        {
            Id = player.Id,
            Nickname = player.Nickname,
            FullName = player.FullName,
            CountryCode = player.CountryCode,
            Role = player.Role.ToString(),
            Rating2 = player.Rating2,
            TotalMapsPlayed = player.TotalMapsPlayed,
            JoinedTeamAtUtc = player.JoinedTeamAtUtc,
            TeamId = player.TeamId
        };
    }
}
