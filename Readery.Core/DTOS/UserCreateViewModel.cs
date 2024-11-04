using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Readery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.DTOS
{
    public class UserCreateViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        // Company selection
        public int? SelectedCompanyId { get; set; } // For storing the selected company ID

        [ValidateNever]
        public List<Company> Companies { get; set; } // List of companies to display in the dropdown

        // List of all available roles
        [ValidateNever]
        public List<string> AllRoles { get; set; }

        // List of selected roles from the form
        public List<string> SelectedRoles { get; set; }
    }

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
