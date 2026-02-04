using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yakudza_docs.Data;
using yakudza_docs.DTOs;
using yakudza_docs.Models;

namespace yakudza_docs.Controllers;

[ApiController]
[Route("api/dishes")]
public class DishesController : ControllerBase
{
    private readonly AppDbContext _context;
    private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB

    public DishesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all dishes with pagination and optional search
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<DishListItemDto>>> GetDishes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        if (page < 1)
        {
            return BadRequest("Page must be greater than or equal to 1");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Page size must be between 1 and 100");
        }

        IQueryable<DishTechCard> query = _context.DishTechCards;

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            // For Cyrillic support, use client-side evaluation with proper culture
            var searchLower = search.ToLower(System.Globalization.CultureInfo.InvariantCulture);

            // Get all ingredients and filter client-side (handles Cyrillic properly)
            var allIngredients = await _context.DishIngredients.ToListAsync();
            var dishIdsWithMatchingIngredients = allIngredients
                .Where(i => i.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains(searchLower))
                .Select(i => i.DishTechCardId)
                .Distinct()
                .ToList();

            // For dish name/description, use server-side search (works for most Cyrillic)
            // Combined with client-side ingredient search results
            query = query.Where(d =>
                d.Name.ToLower().Contains(searchLower) ||
                d.Description.ToLower().Contains(searchLower) ||
                dishIdsWithMatchingIngredients.Contains(d.Id));
        }

        var totalCount = await query.CountAsync();

        var dishes = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DishListItemDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                HasImage = d.Image != null
            })
            .ToListAsync();

        return new PagedResultDto<DishListItemDto>
        {
            Items = dishes,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Get dish details by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DishDetailsDto>> GetDish(int id)
    {
        var dish = await _context.DishTechCards
            .Include(d => d.Ingredients)
            .Where(d => d.Id == id)
            .Select(d => new DishDetailsDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                HasImage = d.Image != null,
                Ingredients = d.Ingredients.Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    WeightGrams = i.WeightGrams
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (dish == null)
        {
            return NotFound($"Dish with ID {id} not found");
        }

        return dish;
    }

    /// <summary>
    /// Get dish image by ID
    /// </summary>
    [HttpGet("{id}/image")]
    public async Task<IActionResult> GetDishImage(int id)
    {
        var dish = await _context.DishTechCards
            .Where(d => d.Id == id)
            .Select(d => new { d.Image })
            .FirstOrDefaultAsync();

        if (dish == null)
        {
            return NotFound($"Dish with ID {id} not found");
        }

        if (dish.Image == null || dish.Image.Length == 0)
        {
            return NotFound("Dish has no image");
        }

        return File(dish.Image, "image/jpeg");
    }

    /// <summary>
    /// Create a new dish
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DishDetailsDto>> CreateDish([FromBody] CreateDishDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate image size if provided
        byte[]? imageBytes = null;
        if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
        {
            try
            {
                imageBytes = Convert.FromBase64String(dto.ImageBase64);
                if (imageBytes.Length > MaxImageSizeBytes)
                {
                    return BadRequest($"Image size exceeds maximum allowed size of {MaxImageSizeBytes / 1024 / 1024}MB");
                }
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Base64 image format");
            }
        }

        var dish = new DishTechCard
        {
            Name = dto.Name,
            Description = dto.Description,
            Image = imageBytes,
            Ingredients = dto.Ingredients.Select(i => new DishIngredient
            {
                Name = i.Name,
                WeightGrams = i.WeightGrams
            }).ToList()
        };

        _context.DishTechCards.Add(dish);
        await _context.SaveChangesAsync();

        var result = new DishDetailsDto
        {
            Id = dish.Id,
            Name = dish.Name,
            Description = dish.Description,
            HasImage = dish.Image != null,
            Ingredients = dish.Ingredients.Select(i => new IngredientDto
            {
                Id = i.Id,
                Name = i.Name,
                WeightGrams = i.WeightGrams
            }).ToList()
        };

        return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, result);
    }

    /// <summary>
    /// Update an existing dish
    /// </summary>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<DishDetailsDto>> UpdateDish(int id, [FromBody] UpdateDishDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var dish = await _context.DishTechCards
            .Include(d => d.Ingredients)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dish == null)
        {
            return NotFound($"Dish with ID {id} not found");
        }

        // Validate image size if provided
        byte[]? imageBytes = null;
        if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
        {
            try
            {
                imageBytes = Convert.FromBase64String(dto.ImageBase64);
                if (imageBytes.Length > MaxImageSizeBytes)
                {
                    return BadRequest($"Image size exceeds maximum allowed size of {MaxImageSizeBytes / 1024 / 1024}MB");
                }
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Base64 image format");
            }
        }

        // Update dish properties
        dish.Name = dto.Name;
        dish.Description = dto.Description;
        if (imageBytes != null)
        {
            dish.Image = imageBytes;
        }

        // Replace ingredients (remove old ones and add new ones)
        _context.DishIngredients.RemoveRange(dish.Ingredients);
        dish.Ingredients = dto.Ingredients.Select(i => new DishIngredient
        {
            DishTechCardId = dish.Id,
            Name = i.Name,
            WeightGrams = i.WeightGrams
        }).ToList();

        await _context.SaveChangesAsync();

        var result = new DishDetailsDto
        {
            Id = dish.Id,
            Name = dish.Name,
            Description = dish.Description,
            HasImage = dish.Image != null,
            Ingredients = dish.Ingredients.Select(i => new IngredientDto
            {
                Id = i.Id,
                Name = i.Name,
                WeightGrams = i.WeightGrams
            }).ToList()
        };

        return result;
    }

    /// <summary>
    /// Delete a dish
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDish(int id)
    {
        var dish = await _context.DishTechCards.FindAsync(id);

        if (dish == null)
        {
            return NotFound($"Dish with ID {id} not found");
        }

        _context.DishTechCards.Remove(dish);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
