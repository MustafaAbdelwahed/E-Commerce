using E_Commerce.Data;
using E_Commerce.Dtos.Categorys_Dtos;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public CategoriesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAll()
        {

            List < GetCategoryDto> getCategories = await _dbContext.Categories
                .AsNoTracking().Select(c => new GetCategoryDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToListAsync();
            return Ok(getCategories);

        }


        [HttpGet("{id:int}")]
        public async Task <IActionResult> GetById(int id)
        {
            GetCategoryDto? category = await _dbContext.Categories.AsNoTracking().Where(c => c.Id == id)
                .Select(c => new GetCategoryDto { Id = c.Id, Name = c.Name, Description = c.Description }).FirstOrDefaultAsync();

            if (category==null)
                 return NotFound("Category not found");
            
            return Ok(category);
        }


        [HttpPost]
        public async Task <IActionResult> Create(CreateCategoryDto dto)
        {
            string name = dto.Name.Trim();

            if (await _dbContext.Categories.AnyAsync(c => c.Name == name))
            {
                return Conflict("Category already exists");
            }

            Category category = new Category { Name = name, Description = dto.Description };

             _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            GetCategoryDto result = new GetCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task <IActionResult> Edit(int id, CreateCategoryDto dto)
        {
            string name = dto.Name.Trim();

            Category? category = await _dbContext.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound("Category not found");
            }


            if (await _dbContext.Categories.AnyAsync(c => c.Name == name && c.Id != id))
            {
                return Conflict("Another category already uses this name");
            }

            category.Name = name;
            category.Description = dto.Description;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }



        [HttpDelete("{id:int}")]
        public async Task <IActionResult> Delete(int id)
        {
            Category? category = await _dbContext.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category==null)
            {
                return NotFound("Category not found");
            }

            if (await _dbContext.Products.AnyAsync(p => p.CategoryId == id))
            {
                return Conflict("Cannot delete a category that has products");
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
