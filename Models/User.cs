using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyDiscgolfDiscs.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
     
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } =string.Empty;
    
    [Required]
    [MinLength(255)]
    public string passwordHash { get; set; }

    public DateTime CreatedDate { get; set; }
    
    

}