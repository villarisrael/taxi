using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificado2.Servicios
{
    public interface IUsuarioRepository
    {
        Task<IdentityResult> AddUserToRoleAsync(string username, string password, string roleName);
        Task<UsuarioCertificados> GetUsuarioByIdAsync(string id);
        Task<UsuarioCertificados> GetUsuarioByUsernameAsync(string username);
        Task<IdentityResult> AddClaimToUserAsync(string username, string claimType, string claimValue);  // Nuevo método para añadir claims
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
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"El usuario '{username}' no fue encontrado."
                });
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!passwordCheck.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidPassword",
                    Description = "La contraseña es incorrecta."
                });
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"El rol '{roleName}' no existe."
                });
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<UsuarioCertificados> GetUsuarioByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<UsuarioCertificados> GetUsuarioByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        // Nuevo método para añadir un claim al usuario
        public async Task<IdentityResult> AddClaimToUserAsync(string username, string claimType, string claimValue)
        {
            // Buscar al usuario por nombre de usuario
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"El usuario '{username}' no fue encontrado."
                });
            }

            // Crear el nuevo claim
            var newClaim = new Claim(claimType, claimValue);

            // Comprobar si el usuario ya tiene este claim
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == claimType && c.Value == claimValue))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ClaimExists",
                    Description = $"El usuario ya tiene el claim '{claimType}' con valor '{claimValue}'."
                });
            }

            // Añadir el claim al usuario
            var result = await _userManager.AddClaimAsync(user, newClaim);
            return result;
        }
    }
}
