using System.ComponentModel.DataAnnotations;

namespace MyDiscgolfDiscs.Models;

public class Disc
{
    [Key] public int Id { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;

    [Required] public DiscCategory Category { get; set; }

    [MaxLength(50)] public string Brand { get; set; } = string.Empty;

    [MaxLength(500)] public string Description { get; set; } = string.Empty;

    // Flight Numbers (Speed, Glide, Turn, Fade)
    public decimal Speed { get; set; }
    public decimal Glide { get; set; }
    public decimal Turn { get; set; }
    public decimal Fade { get; set; }


[MaxLength(20)]
    public string Plastic { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Color { get; set; } = string.Empty;
    
    public int Weight { get; set; } // in grams
    
    [MaxLength(500)]
    public string? ImagePath { get; set; }
    
    // Image positioning properties
    public decimal ImagePositionX { get; set; } = 50; // Default centered (percentage)
    public decimal ImagePositionY { get; set; } = 50; // Default centered (percentage)
    public decimal ImageZoom { get; set; } = 100; // Default 100% zoom
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    // Display property for flight numbers
    public string FlightNumbers => $"{Speed}|{Glide}|{Turn}|{Fade}";
}
