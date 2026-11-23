public class Ticket
{
    public string? Id { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public double MaxPrice { get; set; }
    public double StartingPrice { get; set; }
    public int NumberAvailable { get; set; }
    public int TotalPostings { get; set; }
    public List<Link> Links { get; set; }
}