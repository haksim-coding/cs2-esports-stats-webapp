using cs2_esports.Dtos.Players;
using cs2_esports.Dtos.Teams;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers.Api;

[Route("api/players")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ITeamRepository _teamRepository;

    public PlayersController(IPlayerRepository playerRepository, ITeamRepository teamRepository)
    {
        _playerRepository = playerRepository;
        _teamRepository = teamRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlayerSummaryDto>> GetAll([FromQuery] string? query = null, [FromQuery] int? currentTeamId = null)
    {
        var players = string.IsNullOrWhiteSpace(query)
            ? _playerRepository.GetAllAlphabetical()
            : SearchPlayers(query, currentTeamId);

        return Ok(players.Select(MapToSummaryDto));
    }

    [HttpGet("{id:int}")]
    public ActionResult<PlayerDetailsDto> GetById(int id)
    {
        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        return Ok(MapToDetailsDto(player));
    }

    [HttpPost]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public ActionResult<PlayerDetailsDto> Create([FromBody] PlayerUpsertDto model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        ValidatePlayer(model);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var player = new Player
        {
            Nickname = model.Nickname.Trim(),
            FullName = model.FullName.Trim(),
            CountryCode = model.CountryCode.Trim().ToUpperInvariant(),
            DateOfBirth = model.DateOfBirth,
            Role = model.Role,
            Rating2 = model.Rating2,
            TotalMapsPlayed = model.TotalMapsPlayed,
            TeamId = model.TeamId,
            JoinedTeamAtUtc = DateTime.UtcNow
        };

        _playerRepository.Add(player);
        return CreatedAtAction(nameof(GetById), new { id = player.Id }, MapToDetailsDto(player));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public ActionResult<PlayerDetailsDto> Update(int id, [FromBody] PlayerUpsertDto model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        var existingPlayer = _playerRepository.GetById(id);
        if (existingPlayer is null)
        {
            return NotFound();
        }

        ValidatePlayer(model, id);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        existingPlayer.Nickname = model.Nickname.Trim();
        existingPlayer.FullName = model.FullName.Trim();
        existingPlayer.CountryCode = model.CountryCode.Trim().ToUpperInvariant();
        existingPlayer.DateOfBirth = model.DateOfBirth;
        existingPlayer.Role = model.Role;
        existingPlayer.Rating2 = model.Rating2;
        existingPlayer.TotalMapsPlayed = model.TotalMapsPlayed;
        existingPlayer.TeamId = model.TeamId;

        _playerRepository.Update(existingPlayer);
        return Ok(MapToDetailsDto(existingPlayer));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Delete(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        _playerRepository.Delete(id);
        return NoContent();
    }

    private IReadOnlyList<Player> SearchPlayers(string query, int? currentTeamId)
    {
        var normalizedQuery = query.Trim();
        if (normalizedQuery.Length < 2)
        {
            return [];
        }

        var players = _playerRepository.GetAllAlphabetical()
            .Where(player =>
                player.Nickname.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                player.FullName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                player.CountryCode.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase));

        if (currentTeamId.HasValue)
        {
            players = players.Where(player => !player.TeamId.HasValue || player.TeamId == currentTeamId.Value);
        }

        return players.ToList();
    }

    private void ValidatePlayer(PlayerUpsertDto model, int? currentPlayerId = null)
    {
        var normalizedNickname = model.Nickname.Trim();
        var players = _playerRepository.GetAllAlphabetical();

        if (players.Any(player => player.Id != currentPlayerId && player.Nickname.Equals(normalizedNickname, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Nickname), "A player with this nickname already exists.");
        }

        if (model.TeamId.HasValue && _teamRepository.GetById(model.TeamId.Value) is null)
        {
            ModelState.AddModelError(nameof(model.TeamId), "The selected team could not be found.");
        }
    }

    private PlayerSummaryDto MapToSummaryDto(Player player)
    {
        return new PlayerSummaryDto
        {
            Id = player.Id,
            Nickname = player.Nickname,
            FullName = player.FullName,
            CountryCode = player.CountryCode,
            DateOfBirth = player.DateOfBirth,
            Role = player.Role,
            Rating2 = player.Rating2,
            TotalMapsPlayed = player.TotalMapsPlayed,
            ImagePath = player.ImagePath,
            JoinedTeamAtUtc = player.JoinedTeamAtUtc,
            TeamId = player.TeamId,
            TeamName = player.Team?.Name,
            TeamTag = player.Team?.Tag
        };
    }

    private PlayerDetailsDto MapToDetailsDto(Player player)
    {
        var team = player.TeamId.HasValue ? _teamRepository.GetById(player.TeamId.Value) : null;

        return new PlayerDetailsDto
        {
            Id = player.Id,
            Nickname = player.Nickname,
            FullName = player.FullName,
            CountryCode = player.CountryCode,
            DateOfBirth = player.DateOfBirth,
            Role = player.Role,
            Rating2 = player.Rating2,
            TotalMapsPlayed = player.TotalMapsPlayed,
            ImagePath = player.ImagePath,
            JoinedTeamAtUtc = player.JoinedTeamAtUtc,
            TeamId = player.TeamId,
            TeamName = player.Team?.Name ?? team?.Name,
            TeamTag = player.Team?.Tag ?? team?.Tag,
            CurrentTeam = team is null ? null : new TeamListItemDto
            {
                Id = team.Id,
                Name = team.Name,
                Tag = team.Tag,
                CountryCode = team.CountryCode,
                WorldRanking = team.WorldRanking,
                FoundedYear = team.FoundedYear,
                PrizeMoneyUsd = team.PrizeMoneyUsd,
                LastRosterUpdateUtc = team.LastRosterUpdateUtc,
                PlayerCount = team.Players.Count
            }
        };
    }
}
