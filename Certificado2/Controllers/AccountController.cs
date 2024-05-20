using Certificado2.Modelos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Certificado2.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

     public   AccountController( SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Login()

        {
            return View();
        }

        // Método para manejar la solicitud de inicio de sesión
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Redirigir al usuario a la página de inicio después de iniciar sesión correctamente
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    // Manejar el caso cuando el usuario está bloqueado
                    // Puedes agregar tu lógica aquí
                }
                else
                {
                    // El inicio de sesión falló, mostrar mensaje de error
                    ViewBag.Mensaje = "Usuario o contraseña incorrectos";
                    return View(model);
                }
            }

            // Si llegamos aquí, significa que ocurrió un error de validación, volver a mostrar el formulario con errores
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            // Cierra la sesión del usuario
            await _signInManager.SignOutAsync();

            // Limpia las cookies de autenticación del usuario
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Redirige al usuario a la página de inicio
            return RedirectToAction("Index", "Home");
        }
    }
}
