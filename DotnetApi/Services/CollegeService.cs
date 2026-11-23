using System.Net;
using System.Net.Http.Json;
using DotnetApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DotnetApi.Services;

public interface ICollegeService
{
    Task<IEnumerable<Team>> GetCollegeTeamsAsync(CancellationToken cancellationToken = default);
    Task<Team?> GetCollegeTeamByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CollegeGameSchedule>> GetTeamScheduleAsync(string teamId, int season, CancellationToken cancellationToken = default);

}

public class CollegeService : ICollegeService
{
    private readonly HttpClient httpClient;
    private readonly IMemoryCache cache;
    private const string EspnTeamsEndpoint = "apis/site/v2/sports/football/college-football/teams";
    private const string CacheKey = "CollegeTeams";
    public CollegeService(HttpClient httpClient, IMemoryCache cache)
    {
        this.httpClient = httpClient;
        this.cache = cache;
    }

    public async Task<IEnumerable<Team>> GetCollegeTeamsAsync(CancellationToken cancellationToken = default)
    {
        // Return cached data if available; otherwise fetch and cache it
        var teams = await cache.GetOrCreateAsync<List<Team>>(CacheKey, async entry =>
        {
            // Set cache expiration policy
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            entry.SlidingExpiration = TimeSpan.FromMinutes(20);

            var response = await this.httpClient.GetFromJsonAsync<TeamResponse>(EspnTeamsEndpoint, cancellationToken);

            if (response?.Sports is null)
            {
                // Avoid caching empty results on transient glitches
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                return new List<Team>();
            }

            // Flatten sports -> leagues -> teams -> team into a clean list of Team objects
            return response.Sports
                .SelectMany(sport => sport.Leagues)
                .SelectMany(league => league.Teams)
                .Select(wrapper => wrapper.Team)
                .ToList();
        });

        return teams ?? Enumerable.Empty<Team>();

    }

    public async Task<Team?> GetCollegeTeamByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Reuses the cached list instead of making a second external API call
        var teams = await GetCollegeTeamsAsync(cancellationToken);

        return teams.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<ScheduleResponse?> GetCollegeTeamScheduleByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"TeamSchedule_{id.ToLowerInvariant()}";

        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            // Schedules update with live/recent scores, so use a shorter cache duration
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var endpoint = $"apis/site/v2/sports/football/college-football/teams/{Uri.EscapeDataString(id)}/schedule";
            using var response = await httpClient.GetAsync(endpoint, cancellationToken);

            // Handle invalid team IDs cleanly without throwing an exception
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ScheduleResponse>(cancellationToken: cancellationToken);
        });
    }

    public async Task<IEnumerable<CollegeGameSchedule>> GetTeamScheduleAsync(string teamId, int season, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"FlattenedSchedule_{teamId.ToLowerInvariant()}";

        var games = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            var endpoint = $"apis/site/v2/sports/football/college-football/teams/{Uri.EscapeDataString(teamId)}/schedule?season={season}";
            using var response = await httpClient.GetAsync(endpoint, cancellationToken);

            response.EnsureSuccessStatusCode();

            var scheduleResponse = await response.Content.ReadFromJsonAsync<ScheduleResponse>(cancellationToken: cancellationToken);

            if (scheduleResponse?.Events is null)
            {
                return new List<CollegeGameSchedule>();
            }

            return scheduleResponse.Events
                .Select(MapToFlattenedGame)
                .ToList();
        });

        return games ?? Enumerable.Empty<CollegeGameSchedule>();
    }

    private static CollegeGameSchedule MapToFlattenedGame(Event evt)
    {
        var competition = evt.Competitions.FirstOrDefault();
        var home = competition?.Competitors.FirstOrDefault(c => string.Equals(c.HomeAway, "home", StringComparison.OrdinalIgnoreCase));
        var away = competition?.Competitors.FirstOrDefault(c => string.Equals(c.HomeAway, "away", StringComparison.OrdinalIgnoreCase));

        var homeTeamName = ResolveTeamName(home?.Team);
        var awayTeamName = ResolveTeamName(away?.Team);

        // Determine the winner name if the game has a winner flag set
        string? winnerName = null;
        if (home?.Winner == true)
        {
            winnerName = homeTeamName;
        }
        else if (away?.Winner == true)
        {
            winnerName = awayTeamName;
        }

        // Format score as "HomeScore - AwayScore" if scores are available (null for upcoming games)
        string? formattedScore = (home?.Score is not null && away?.Score is not null)
            ? $"{home.Score.DisplayValue} - {away.Score.DisplayValue}"
            : null;

        return new CollegeGameSchedule
        {
            Week = evt.Week?.Text ?? string.Empty,
            SeasonType = evt.SeasonType?.Name ?? string.Empty,
            Stadium = competition?.Venue?.FullName ?? string.Empty,
            Attendance = competition?.Attendance ?? 0,
            Date = competition?.Date ?? evt.Date,
            HomeTeam = homeTeamName,
            HomeTeamAbbrev = home?.Team?.ShortDisplayName ?? string.Empty,
            AwayTeam = awayTeamName,
            AwayTeamAbbrev = away?.Team?.ShortDisplayName ?? string.Empty,
            Winner = winnerName,
            Score = formattedScore
        };
    }
    private static string ResolveTeamName(Team? competitor)
    {
        if (competitor is null) return string.Empty;

        // schedule competitors populate Nickname/ShortDisplayName rather than Name
        if (!string.IsNullOrWhiteSpace(competitor.DisplayName)) return competitor.DisplayName;
        if (!string.IsNullOrWhiteSpace(competitor.Nickname)) return competitor.Nickname;
        return competitor.ShortDisplayName ?? string.Empty;
    }
}
