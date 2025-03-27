using Certificado2.Modelos;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IRepositorioClientes _repositorioClientes;

        public ClientesController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IRepositorioClientes repocl)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _repositorioClientes = repocl;
        }


        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarClienteAsync([FromBody] ClienteRequest cliente)
        {
            if (cliente == null || string.IsNullOrEmpty(cliente.Correo) || string.IsNullOrEmpty(cliente.Password))
            {
                return BadRequest(new { mensaje = "Correo y contraseña son obligatorios." });
            }



            var result = await _repositorioClientes.CrearClienteAsync(cliente.Nombre, cliente.Apellido, cliente.Correo, cliente.Telefono,cliente.Password);

            if (!result)
            {
                return BadRequest(new { mensaje = "Error al registrar usuario" });
            }

            return Ok(new { mensaje = "Cliente registrado correctamente" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { mensaje = "Usuario y contraseña son obligatorios." });
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            return Ok(new { mensaje = "Inicio de sesión exitoso", usuario = user.Id });
        }


    }
}
