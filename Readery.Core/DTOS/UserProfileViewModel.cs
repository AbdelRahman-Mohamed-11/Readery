using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.DTOS
{
    public class UserProfileViewModel
    {
        public string Name { get; set; }

        [ValidateNever]
        public string PhotoUrl { get; set; }
        public string? NewPassword { get; set; }

    }
}
