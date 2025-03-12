using Certificado2.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly SignInManager<UsuarioCertificados> _signInManager;
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly IUsuarioRepository _userRepository;
        private readonly IRepositorioOrganizaciones _repositorioOrganizaciones;

        public UsuariosController(
            UserManager<UsuarioCertificados> userManager,
            SignInManager<UsuarioCertificados> signInManager,
            IUsuarioRepository userRepository,
            IRepositorioOrganizaciones repositorioOrganizaciones)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _repositorioOrganizaciones = repositorioOrganizaciones;
        }

        [HttpPost("verificarusuario")]
        public async Task<ActionResult<UsuarioResponse>> VerificarUsuario([FromBody] LoginRequest login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized();

            var usuario = await _userManager.FindByNameAsync(login.UserName);
            if (usuario == null)
                return Unauthorized();

            // Obtener el rol del usuario (como solo puede tener uno, tomamos el primero)
            var roles = await _userManager.GetRolesAsync(usuario);
            string rol = roles.FirstOrDefault(); // Retorna null si no tiene rol asignado

            // Retornar usuario con su rol
            var response = new UsuarioResponse
            {
                Id = usuario.Id,
                Nombre = usuario.NombreCompleto,
                Correo = usuario.Email,
                Rol = rol // Almacena el único rol que puede tener el usuario
            };

            return Ok(response);
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UsuarioResponse
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; } // Ahora es un único string en lugar de una lista
    }
}

