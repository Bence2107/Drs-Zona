namespace Entities.Models.Standings;

public class Driver
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Nationality { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public int DriverNumber { get; set; }
}