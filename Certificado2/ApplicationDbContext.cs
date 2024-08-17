
        using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;



    namespace  Certificado2
{
        public class ApplicationDbContext : IdentityDbContext<UsuarioCertificados>
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }
        }

    public class UsuarioCertificados : IdentityUser
    {
        public int idcertificador { get; set; }

        public string? NombreCompleto { get; set; }

    }

}


