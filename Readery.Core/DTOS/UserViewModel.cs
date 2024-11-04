using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.DTOS
{
    public class UserViewModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public bool IsActive { get; set; }

    }
}
