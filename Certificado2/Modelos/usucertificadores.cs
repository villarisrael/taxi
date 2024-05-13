namespace Certificado2.Modelos
{
    public class UsuCertificadores
    {
        // Properties corresponding to the table columns
        public int Id { get; set; } // Auto-incremented primary key
        public string Nombre { get; set; } // Name of the certifier
        public string Usuario { get; set; } // Username
        public string Password { get; set; } // Password
        public string Email { get; set; } // Email address
        public string WhatsApp { get; set; } // WhatsApp number
        public int? IdCentificador { get; set; } // Index referencing another table

        // Constructor
        public UsuCertificadores()
        {
            // Default constructor
        }

        // Additional methods, if needed
    }

}
