using System.ComponentModel.DataAnnotations;

namespace Certificado2.Modelos
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

    
    }

    public class LoginRequest
    {
        public string UserName { get; set; }  // Puede ser Email o Usuario
        public string Password { get; set; }
    }

    public class ConductorRequest
    {
        public string UserName { get; set; }  // Puede ser el correo o usuario
        public string Password { get; set; }
    }



}
