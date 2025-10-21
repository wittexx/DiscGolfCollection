using Microsoft.AspNetCore.Mvc;
using MyDiscgolfDiscs.Models;
using MyDiscgolfDiscs.Services;

namespace MyDiscgolfDiscs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscsController : ControllerBase
{
    private readonly DiscService _discService;

    public DiscsController(DiscService discService)
    {
        _discService = discService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Disc>>> GetAllDiscs()
    {
        var discs = await _discService.GetAllDiscsAsync();
        return Ok(discs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Disc>> GetDisc(int id)
    {
        var disc = await _discService.GetDiscByIdAsync(id);
        if (disc == null)
        {
            return NotFound();
        }
        return Ok(disc);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<Disc>>> GetDiscsByCategory(DiscCategory category)
    {
        var discs = await _discService.GetDiscsByCategoryAsync(category);
        return Ok(discs);
    }

    [HttpPost]
    public async Task<ActionResult<Disc>> CreateDisc(Disc disc)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdDisc = await _discService.AddDiscAsync(disc);
        return CreatedAtAction(nameof(GetDisc), new { id = createdDisc.Id }, createdDisc);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDisc(int id, Disc disc)
    {
        if (id != disc.Id)
        {
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _discService.UpdateDiscAsync(disc);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDisc(int id)
    {
        var result = await _discService.DeleteDiscAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadDiscImage(int id, IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("No image provided");
        }

        var disc = await _discService.GetDiscByIdAsync(id);
        if (disc == null)
        {
            return NotFound("Disc not found");
        }

        // Check file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
        if (!allowedTypes.Contains(image.ContentType))
        {
            return BadRequest("Invalid image format. Only JPEG, PNG, and GIF are allowed.");
        }

        try
        {
            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            var fileExtension = Path.GetExtension(image.FileName);
            var fileName = $"disc_{id}_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";
            var filePath = Path.Combine(imageDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Update disc with image path
            disc.ImagePath = $"/images/{fileName}";
            await _discService.UpdateDiscAsync(disc);

            return Ok(new { imagePath = disc.ImagePath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading image: {ex.Message}");
        }
    }

    [HttpPatch("{id}/image-position")]
    public async Task<IActionResult> UpdateImagePosition(int id, [FromBody] ImagePositionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var disc = await _discService.GetDiscByIdAsync(id);
            if (disc == null)
            {
                return NotFound();
            }

            disc.ImagePositionX = request.ImagePositionX;
            disc.ImagePositionY = request.ImagePositionY;
            disc.ImageZoom = request.ImageZoom;

            await _discService.UpdateDiscAsync(disc);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpGet("categories")]
    public ActionResult<object[]> GetCategories()
    {
        var categories = Enum.GetValues<DiscCategory>()
            .Select(c => new { id = (int)c, name = c.ToString() })
            .ToArray();
        return Ok(categories);
    }
}
