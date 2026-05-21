using cs2_esports.Models;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Data;

public class Cs2ScopeDbContext : DbContext
{
    public Cs2ScopeDbContext(DbContextOptions<Cs2ScopeDbContext> options) : base(options)
    {
    }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<EventVenue> EventVenues => Set<EventVenue>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Event> Tournaments => Set<Event>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchMap> MatchMaps => Set<MatchMap>();
    public DbSet<ForumUser> ForumUsers => Set<ForumUser>();
    public DbSet<Forum> Forums => Set<Forum>();
    public DbSet<ForumComment> ForumComments => Set<ForumComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.UseTptMappingStrategy();
            entity.Property(user => user.Password).IsRequired();

            entity.HasIndex(user => user.Username).IsUnique();
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<ForumUser>(entity =>
        {
            entity.ToTable("ForumUsers");
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.ToTable("AdminUsers");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(team => team.PrizeMoneyUsd).HasPrecision(18, 2);

            entity.HasMany(team => team.Players)
                .WithOne(player => player.Team)
                .HasForeignKey(player => player.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(team => team.Tournaments)
                .WithMany(tournament => tournament.Teams)
                .UsingEntity<Dictionary<string, object>>(
                    "EventTeams",
                    right => right.HasOne<Event>()
                        .WithMany()
                        .HasForeignKey("TournamentsId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Team>()
                        .WithMany()
                        .HasForeignKey("TeamsId")
                        .OnDelete(DeleteBehavior.Cascade));

            entity.HasMany(team => team.HomeMatches)
                .WithOne(match => match.TeamA)
                .HasForeignKey(match => match.TeamAId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(team => team.AwayMatches)
                .WithOne(match => match.TeamB)
                .HasForeignKey(match => match.TeamBId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.SharedTypeEntity<Dictionary<string, object>>("EventTeams").HasData(
            new { TeamsId = 7, TournamentsId = 1 },
            new { TeamsId = 8, TournamentsId = 1 },
            new { TeamsId = 9, TournamentsId = 1 },
            new { TeamsId = 10, TournamentsId = 1 },
            new { TeamsId = 11, TournamentsId = 1 },
            new { TeamsId = 12, TournamentsId = 1 },
            new { TeamsId = 7, TournamentsId = 2 },
            new { TeamsId = 8, TournamentsId = 2 },
            new { TeamsId = 9, TournamentsId = 2 },
            new { TeamsId = 10, TournamentsId = 2 },
            new { TeamsId = 11, TournamentsId = 2 },
            new { TeamsId = 12, TournamentsId = 2 },
            new { TeamsId = 7, TournamentsId = 3 },
            new { TeamsId = 8, TournamentsId = 3 },
            new { TeamsId = 9, TournamentsId = 3 },
            new { TeamsId = 10, TournamentsId = 3 },
            new { TeamsId = 11, TournamentsId = 3 },
            new { TeamsId = 12, TournamentsId = 3 },
            new { TeamsId = 1, TournamentsId = 4 },
            new { TeamsId = 4, TournamentsId = 4 },
            new { TeamsId = 7, TournamentsId = 4 },
            new { TeamsId = 8, TournamentsId = 4 },
            new { TeamsId = 2, TournamentsId = 5 },
            new { TeamsId = 3, TournamentsId = 5 },
            new { TeamsId = 5, TournamentsId = 5 },
            new { TeamsId = 7, TournamentsId = 5 },
            new { TeamsId = 8, TournamentsId = 5 },
            new { TeamsId = 1, TournamentsId = 6 },
            new { TeamsId = 2, TournamentsId = 6 },
            new { TeamsId = 4, TournamentsId = 6 },
            new { TeamsId = 6, TournamentsId = 6 },
            new { TeamsId = 7, TournamentsId = 6 },
            new { TeamsId = 8, TournamentsId = 6 },
            new { TeamsId = 3, TournamentsId = 7 },
            new { TeamsId = 5, TournamentsId = 7 },
            new { TeamsId = 6, TournamentsId = 7 },
            new { TeamsId = 7, TournamentsId = 7 },
            new { TeamsId = 8, TournamentsId = 7 });

        modelBuilder.Entity<ForumUser>(entity =>
        {
            entity.HasMany(user => user.FavoriteTeams)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ForumUserFavoriteTeams",
                    right => right.HasOne<Team>()
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<ForumUser>()
                        .WithMany()
                        .HasForeignKey("ForumUserId")
                        .OnDelete(DeleteBehavior.Cascade));

            entity.HasMany(user => user.FavoritePlayers)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ForumUserFavoritePlayers",
                    right => right.HasOne<Player>()
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<ForumUser>()
                        .WithMany()
                        .HasForeignKey("ForumUserId")
                        .OnDelete(DeleteBehavior.Cascade));
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(eventItem => eventItem.PrizePoolUsd).HasPrecision(18, 2);

            entity.HasMany(eventItem => eventItem.Matches)
                .WithOne(match => match.Event)
                .HasForeignKey(match => match.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(eventItem => eventItem.ForumThreads)
                .WithOne(forum => forum.Event)
                .HasForeignKey(forum => forum.TournamentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(eventItem => eventItem.EventVenue)
                .WithMany(venue => venue.Tournaments)
                .HasForeignKey(eventItem => eventItem.EventVenueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(eventItem => eventItem.AdminUser)
                .WithMany(admin => admin.ManagedTournaments)
                .HasForeignKey(eventItem => eventItem.AdminUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Forum>(entity =>
        {
            entity.HasMany(forum => forum.Comments)
                .WithOne(comment => comment.Forum)
                .HasForeignKey(comment => comment.ForumId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(forum => forum.Author)
                .WithMany(author => author.Threads)
                .HasForeignKey(forum => forum.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ForumComment>(entity =>
        {
            entity.HasOne(comment => comment.Author)
                .WithMany(author => author.Comments)
                .HasForeignKey(comment => comment.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(player => player.Rating2).HasPrecision(3, 2);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasMany(match => match.Maps)
                .WithOne(map => map.Match)
                .HasForeignKey(map => map.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(match => new { match.EventId, match.ScheduledAtUtc });
        });

        modelBuilder.Entity<MatchMap>(entity =>
        {
            entity.HasIndex(matchMap => new { matchMap.MatchId, matchMap.MapSequence }).IsUnique();
        });

        modelBuilder.Entity<Team>().HasData(MatchSeedData.GetTeams());
        modelBuilder.Entity<Player>().HasData(MatchSeedData.GetPlayers());
        modelBuilder.Entity<Match>().HasData(MatchSeedData.GetMatches());
        modelBuilder.Entity<MatchMap>().HasData(MatchSeedData.GetMatchMaps());
    }
}