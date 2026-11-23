public class League
{
    public string? Abbreviation { get; set; }
    public string? Id { get; set; }
    public string? Name { get; set; }
    public Season? Season { get; set; }
    public string? ShortName { get; set; }
    public string? Slug { get; set; }
    public List<Teams> Teams { get; set; }
    public string? Uid { get; set; }
    public int? Year { get; set; }
}