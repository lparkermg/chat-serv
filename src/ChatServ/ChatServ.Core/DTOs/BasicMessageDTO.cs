using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServ.Core.Models
{
    public class BasicMessageDTO
    {
        public required string Sender { get; set; }

        public required int Content { get; set; }
    }
}
