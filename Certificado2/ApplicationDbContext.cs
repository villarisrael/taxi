
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

   

}


