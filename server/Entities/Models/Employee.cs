using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Employee
{
    [Key]
    [Column("id")]
    public int Id { get; init; }
    
    [Column("f_name")]
    public string? FirstName { get; set; }
    
    [Column("l_name")]
    public string? LastName { get; set; }
}