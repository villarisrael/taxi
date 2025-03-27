using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certificado2.Modelos
{
    public class ClienteRequest
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
    }
}
