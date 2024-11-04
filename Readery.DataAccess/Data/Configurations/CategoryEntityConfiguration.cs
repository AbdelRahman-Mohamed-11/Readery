using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Readery.Web.Models;

namespace Readery.Web.Data.Configurations
{
    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {


            builder.ToTable(tb => tb.HasCheckConstraint("CK_Category_DisplayOrder", "[DisplayOrder] >= 0 AND [DisplayOrder] <= 100"));

        }
    }
}
