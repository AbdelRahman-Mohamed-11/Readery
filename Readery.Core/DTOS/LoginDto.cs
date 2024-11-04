using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.DTOS
{
    public class LoginDto
    {
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$",
            ErrorMessage = "Password must be at least 6 characters long, contain one lowercase, one uppercase letter, one number, and one special character")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
