using Readery.Core.Models;


namespace Readery.Web.Models;

public class Category : BaseEntity
{
    public string Name { get; set; }

    public int DisplayOrder { get; set; }
}
