using System.ComponentModel.DataAnnotations;

namespace Readery.Web.DTOS.Category.Update
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Range(0, 100, ErrorMessage = "the range must be between 0 and 100")]
        public int DisplayOrder { get; set; }
    }
}
