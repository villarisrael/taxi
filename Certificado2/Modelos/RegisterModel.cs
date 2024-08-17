using System.ComponentModel.DataAnnotations;

namespace Certificado2.Modelos
{
   
        public class RegisterViewModel
        {
            [Required]
            [Display(Name = "Nombre de Usuario")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Correo Electrónico")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Nombre Completo")]
            public string NombreCompleto { get; set; }

            [Display(Name = "ID Certificador")]
            public int idcertificador { get; set; }

            public string PhoneNumber { get; set; }
        }
    
}
