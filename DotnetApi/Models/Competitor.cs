using DotnetApi.Models;

public class Competitor
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public int? Order { get; set; }
    public string? HomeAway { get; set; }
    public bool? Winner { get; set; }
    public Team? Team { get; set; }
    public Score Score { get; set; }
    public List<Leader> Leaders { get; set; }
    public List<Record> Record { get; set; }
    public CuratedRank CuratedRank { get; set; }
}