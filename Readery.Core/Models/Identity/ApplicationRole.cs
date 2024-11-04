using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.Models.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole()
        {

        }
        public ApplicationRole(string name)
        {
            this.Name = name;
        }
    }
}
