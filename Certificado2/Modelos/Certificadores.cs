
using System;

namespace Certificado2.Modelos
{
   

    public class Certificadores
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; }
        public string NombreResponsable { get; set; }
        public string Email { get; set; }
        public string CP { get; set; }
        public string Telefono { get; set; }
        public string EmailFacturacion { get; set; }
        public string RFC { get; set; }
        public byte[] Logo { get; set; }

    }
}
