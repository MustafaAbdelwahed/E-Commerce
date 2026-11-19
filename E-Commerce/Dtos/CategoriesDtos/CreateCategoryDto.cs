using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Dtos.Categorys_Dtos
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
   
}
