public class Event
{
    public string? Id { get; set; }
    public string? Date { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public Season? Season { get; set; }
    public SeasonType? SeasonType { get; set; }
    public Week? Week { get; set; }
    public bool TimeValid { get; set; }
    public List<Competition> Competitions { get; set; }
    public List<Link> Links { get; set; }
}