public class Competition
{
    public string Id { get; set; }
    public string? Date { get; set; }
    public int Attendance { get; set; }
    public ScheduleType? Type { get; set; }
    public bool TimeValid { get; set; }
    public bool NeutralSite { get; set; }
    public bool BoxscoreAvailable { get; set; }
    public bool TicketsAvailable { get; set; }
    public Venue Venue { get; set; }
    public List<Competitor> Competitors { get; set; }
    public List<object> Notes { get; set; }
    public List<Broadcast> Broadcasts { get; set; }
    public Status Status { get; set; }
    public List<Ticket> Tickets { get; set; }
}