using Microsoft.AspNetCore.Identity;


namespace Readery.Core.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public int? CompanyId { get; set; }

        public Company Company { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ProfilePhotoUrl { get; set; }
    }
}
