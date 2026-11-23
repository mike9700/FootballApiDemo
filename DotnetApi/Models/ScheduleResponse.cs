using DotnetApi.Models;

public class ScheduleResponse
{
    public DateTime Timestamp { get; set; }
    public string? Status { get; set; }
    public Season? Season { get; set; }
    public Team? Team { get; set; }
    public List<Event>? Events { get; set; }
    public RequestedSeason? RequestedSeason { get; set; }
}