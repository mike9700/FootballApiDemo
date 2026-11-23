namespace DotnetApi.Models;


public class Team
{
    public string? Abbreviation { get; set; }
    public string? AlternateColor { get; set; }
    public string? Color { get; set; }
    public string? DisplayName { get; set; }
    public string? Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsAllStar { get; set; }
    public List<Link> Links { get; set; }
    public string? Location { get; set; }
    public List<Logo> Logos { get; set; }
    public string? Name { get; set; }
    public string? Nickname { get; set; }
    public string? ShortDisplayName { get; set; }
    public string? Slug { get; set; }
    public string? Uid { get; set; }
}