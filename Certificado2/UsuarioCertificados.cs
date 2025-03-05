using Microsoft.AspNetCore.Identity;

namespace Certificado2
{
    public class UsuarioCertificados : IdentityUser
    {
       public int idOrganizacion { get; set; }

        public string? NombreCompleto { get; set; }

    }
}
