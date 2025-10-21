using Microsoft.EntityFrameworkCore;
using MyDiscgolfDiscs.Models;

namespace MyDiscgolfDiscs.Services;

public class DiscService(DiscContext context)
{
    public async Task InitializeDatabaseAsync()
    {
        await context.Database.EnsureCreatedAsync();
    }

    public async Task<IEnumerable<Disc>> GetAllDiscsAsync()
    {
        return await context.Discs.OrderBy(d => d.Category).ThenBy(d => d.Name).ToListAsync();
    }

    public async Task<IEnumerable<Disc>> GetDiscsByCategoryAsync(DiscCategory category)
    {
        return await context.Discs
            .Where(d => d.Category == category)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Disc?> GetDiscByIdAsync(int id)
    {
        return await context.Discs.FindAsync(id);
    }

    public async Task<Disc> AddDiscAsync(Disc disc)
    {
        context.Discs.Add(disc);
        await context.SaveChangesAsync();
        return disc;
    }

    public async Task<Disc> UpdateDiscAsync(Disc disc)
    {
        // Get the existing disc from database
        var existingDisc = await context.Discs.FindAsync(disc.Id);
        if (existingDisc == null)
        {
            throw new ArgumentException("Disc not found", nameof(disc));
        }

        // Update only the properties we want to change
        existingDisc.Name = disc.Name;
        existingDisc.Category = disc.Category;
        existingDisc.Brand = disc.Brand;
        existingDisc.Description = disc.Description;
        existingDisc.Speed = disc.Speed;
        existingDisc.Glide = disc.Glide;
        existingDisc.Turn = disc.Turn;
        existingDisc.Fade = disc.Fade;
        existingDisc.Plastic = disc.Plastic;
        existingDisc.Color = disc.Color;
        existingDisc.Weight = disc.Weight;
        
        // Only update ImagePath if provided (don't overwrite with empty string)
        if (!string.IsNullOrEmpty(disc.ImagePath))
        {
            existingDisc.ImagePath = disc.ImagePath;
        }

        // Save changes
        await context.SaveChangesAsync();
        return existingDisc;
    }

    public async Task<bool> DeleteDiscAsync(int id)
    {
        var disc = await context.Discs.FindAsync(id);
        if (disc == null) return false;

        // Delete associated image file if it exists
        if (!string.IsNullOrEmpty(disc.ImagePath) && File.Exists(disc.ImagePath))
        {
            try
            {
                File.Delete(disc.ImagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not delete image file: {ex.Message}");
            }
        }

        context.Discs.Remove(disc);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<string> SaveDiscImageAsync(string sourceImagePath, int discId)
    {
        if (!File.Exists(sourceImagePath))
            throw new FileNotFoundException("Source image file not found.");

        var imageDirectory = Path.Combine(Environment.CurrentDirectory, "Images");
        if (!Directory.Exists(imageDirectory))
            Directory.CreateDirectory(imageDirectory);

        var fileExtension = Path.GetExtension(sourceImagePath);
        var fileName = $"disc_{discId}_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";
        var destinationPath = Path.Combine(imageDirectory, fileName);

        File.Copy(sourceImagePath, destinationPath, true);
        return Task.FromResult(destinationPath);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
