using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.DTOS
{
    public class UserEditViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public List<string> SelectedRoles { get; set; }

        [ValidateNever]
        public List<string> AllRoles { get; set; }
    }
}
