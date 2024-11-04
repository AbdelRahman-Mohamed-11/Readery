using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Readery.Utilities.EmailModels
{
    public class EmailMessage
    {
        public EmailMessage() { }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string Name { get; set; }
    }

}
