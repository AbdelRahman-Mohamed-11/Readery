using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Core.Models
{
    public class BaseEntity
    {
        [ValidateNever]
        public int Id { get; set; }
    }
}
