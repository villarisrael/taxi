
using Microsoft.AspNetCore.Identity;





namespace Certificado2.Servicios
{

    public interface IUsuarioRepository
    {
        Task<IdentityResult> AddUserToRoleAsync(string username, string password, string roleName);
    }
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly SignInManager<UsuarioCertificados> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioRepository(UserManager<UsuarioCertificados> userManager,
                              SignInManager<UsuarioCertificados> signInManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserToRoleAsync(string username, string password, string roleName)
        {
            // Buscar al usuario por nombre de usuario
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
            }

            // Verificar la contraseña del usuario
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!passwordCheck.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Contraseña incorrecta" });
            }

            // Verificar si el rol existe
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Rol no encontrado" });
            }

            // Asignar el rol al usuario
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result;
        }
    }
}
