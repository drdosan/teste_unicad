using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Domain.Entities
{
    public class UsersAAWebDTO
    {
        public string Name { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string EmailTransportodora { get; set; }
        public int? CodigoTransportodora { get; set; }
    }
}
