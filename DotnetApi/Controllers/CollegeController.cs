using DotnetApi.Models;
using DotnetApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("api/College-Team")]
public class CollegeTeamController : ControllerBase
{
    private readonly ICollegeService collegeService;

    public CollegeTeamController(ICollegeService collegeTeamService)
    {
        this.collegeService = collegeTeamService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IEnumerable<Team>>> GetCollegeTeams(CancellationToken cancellationToken)
    {
        try
        {
            var teams = await this.collegeService.GetCollegeTeamsAsync(cancellationToken);
            return Ok(teams);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "Failed to retrieve college teams from API.",
                detail = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetCollegeTeamById(string id, CancellationToken cancellationToken)
    {
        try
        {
            var team = await collegeService.GetCollegeTeamByIdAsync(id, cancellationToken);

            if (team is null)
            {
                return NotFound(new { message = $"College team with ID '{id}' was not found." });
            }

            return Ok(team);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "Failed to retrieve college teams from API.",
                detail = ex.Message
            });
        }
    }

    [HttpGet("{teamId}/schedule")]
    public async Task<ActionResult<IEnumerable<CollegeGameSchedule>>> GetCollegeTeamSchedule(string teamId, int? season, CancellationToken cancellationToken)
    {
        try
        {
            if (!season.HasValue)
            {
                season = DateTime.Now.Month >= 8 ? DateTime.Now.Year : DateTime.Now.Year - 1;
            }
            var schedule = await this.collegeService.GetTeamScheduleAsync(teamId, season.Value, cancellationToken);

            if (schedule is null)
            {
                return NotFound(new { message = $"Schedule for team ID '{teamId}' was not found." });
            }

            return Ok(schedule);
        }
        catch (HttpRequestException ex)
        {
            // default to 502 Bad Gateway for network/connection failures.
            var statusCode = ex.StatusCode.HasValue
                ? (int)ex.StatusCode.Value
                : StatusCodes.Status502BadGateway;

            return StatusCode(statusCode, new
            {
                message = "Failed to retrieve team schedule from ESPN API.",
                detail = ex.Message
            });
        }
    }
}
