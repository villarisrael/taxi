using Microsoft.AspNetCore.Identity;

namespace Certificado2
{
    public class UsuarioCertificados : IdentityUser
    {
        public int idcertificador { get; set; }

        public string? NombreCompleto { get; set; }

    }
}
