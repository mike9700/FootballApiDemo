public class CollegeGameSchedule
{
    public string Week { get; set; } = string.Empty;
    public string SeasonType { get; set; } = string.Empty;
    public string Stadium { get; set; } = string.Empty;
    public int Attendance { get; set; }
    public string Date { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string HomeTeamAbbrev { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string AwayTeamAbbrev { get; set; } = string.Empty;
    public string? Winner { get; set; }
    public string? Score { get; set; }
}