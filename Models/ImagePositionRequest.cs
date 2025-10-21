using System.ComponentModel.DataAnnotations;

namespace MyDiscgolfDiscs.Models;

public class ImagePositionRequest
{
    [Range(0, 100, ErrorMessage = "Image position X must be between 0 and 100")]
    public decimal ImagePositionX { get; set; }
    
    [Range(0, 100, ErrorMessage = "Image position Y must be between 0 and 100")]
    public decimal ImagePositionY { get; set; }
    
    [Range(50, 200, ErrorMessage = "Image zoom must be between 50 and 200")]
    public decimal ImageZoom { get; set; }
}
