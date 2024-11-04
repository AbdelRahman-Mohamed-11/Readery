using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Readery.Web.DTOS.Category.Create
{

    public class CreateCategoryDto
    {
        public string Name { get; set; }

        [Range(0, 100, ErrorMessage = "the range must be between 0 and 100")]
        public int DisplayOrder { get; set; }
    }
}
