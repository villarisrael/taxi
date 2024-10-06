using Certificado2.Modelos;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Certificado2.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<UsuarioCertificados> _signInManager;
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly IUsuarioRepository _userRepository;
        private readonly IRepositorioCertificadores _repositorioCertificadores;

        public AccountController(UserManager<UsuarioCertificados> userManager, SignInManager<UsuarioCertificados> signInManager, IUsuarioRepository userRepository, IRepositorioCertificadores repositorioCertificadores)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _repositorioCertificadores = repositorioCertificadores;
        }

        public IActionResult Login()

        {
            return View();
        }

        // Método para manejar la solicitud de inicio de sesión
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)

        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            model.RememberMe = false;
            if (ModelState.IsValid)
            {

              Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
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


        public IActionResult Register()
        {
            return View();
        }

        // Acción para manejar el formulario de registro
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Crea una instancia del usuario
                var user = new UsuarioCertificados
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    NombreCompleto = model.NombreCompleto,
                    idcertificador = model.idcertificador, // Establece idcertificador desde el modelo de vista
                    PhoneNumber = model.PhoneNumber,
                    
                };

                // Intenta crear el usuario
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    var resultrole = await _userRepository.AddUserToRoleAsync(model.UserName, model.Password, "Admin");

                    if (resultrole.Succeeded)
                    {
                        Console.WriteLine("Rol asignado correctamente");
                    }
                    else
                    {
                        Console.WriteLine("No pude añadir el rol ");
                    }

                    // Redirige al usuario a la página de inicio o al inicio de sesión
                    return RedirectToAction("Login", "Account");
                }

                // Agrega errores al modelo si la creación del usuario falla
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si el modelo no es válido, vuelve a mostrar el formulario con errores
            return View(model);
        }


        public IActionResult RegisterUsuCer(int idcertificador)
        {
            ViewBag.idcertificador = idcertificador;
            return View();
        }

        // Acción para manejar el formulario de registro
        [HttpPost]
        public async Task<IActionResult> RegisterUsuCer(RegisterViewModel model)
        {
            ViewBag.Mensaje = "";
            if (ModelState.IsValid)
            {
                // Crea una instancia del usuario
                var user = new UsuarioCertificados
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    NombreCompleto = model.NombreCompleto,
                    idcertificador = model.idcertificador // Establece idcertificador desde el modelo de vista
                };

                Certificadores certificador = await _repositorioCertificadores.ObtenerDetalleAsync(model.idcertificador);
                string tieneaccesojoyeria = "NO";
                string tieneaccesoartesania = "NO";
                string tieneaccesonumismatica = "NO";
                if (certificador != null)
                {
                    if (certificador.joyeria)
                    {
                        tieneaccesojoyeria = "SI";
                    }
                    else
                    {
                        tieneaccesojoyeria = "NO";
                    }
                    if (certificador.artesania)
                    {
                        tieneaccesoartesania = "SI";
                    }
                    else
                    {
                        tieneaccesoartesania = "NO";
                    }
                    if (certificador.numismatica)
                    {
                        tieneaccesonumismatica = "SI";
                    }
                    else
                    {
                        tieneaccesonumismatica = "NO";
                    }


                }

                // Intenta crear el usuario
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Redirige al usuario a la página de inicio o al inicio de sesión

                    var resultrole = await _userRepository.AddUserToRoleAsync(model.UserName, model.Password, "Certificador");
                    var declaracionjoyeria = await _userRepository.AddClaimToUserAsync(model.UserName, "JOYERIA", tieneaccesojoyeria);
                    var declaracionartesania = await _userRepository.AddClaimToUserAsync(model.UserName, "ARTESANIA", tieneaccesoartesania);
                    var declaracionnumismatica = await _userRepository.AddClaimToUserAsync(model.UserName, "NUMISMATICA", tieneaccesonumismatica);

                    if (resultrole.Succeeded)
                    {
                        Console.WriteLine("Rol asignado correctamente");
                    }
                    else
                    {
                        Console.WriteLine("No pude añadir el rol ");
                    }



                    return RedirectToAction("Login", "Account");
                }

                // Agrega errores al modelo si la creación del usuario falla
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else {
                ViewBag.Mensaje = ModelState.ValidationState;
                return View(model); 
            }

            // Si el modelo no es válido, vuelve a mostrar el formulario con errores
            return View(model);
        }
    }


}
