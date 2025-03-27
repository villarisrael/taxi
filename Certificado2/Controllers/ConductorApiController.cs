using Certificado2.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    [ApiController]
    [Route("api/conductorapi")]
    public class ConductorApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly IConfiguration _configuration;
        private const double RadioTierraKm = 6371;

        public ConductorApiController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Inicia sesión y valida credenciales del conductor con Identity.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ConductorRequest request)
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

            return Ok(new { mensaje = "Inicio de sesión exitoso", usuario = user.UserName });
        }

        [HttpGet("cercanos")]
        public async Task<IActionResult> GetConductoresCercanos([FromQuery] double? latitud, [FromQuery] double? longitud)
        {
            if (latitud == null || longitud == null)
                return BadRequest(new { error = "Faltan datos requeridos: latitud y longitud" });

            var conductoresCercanos = new List<object>();
            var connectionString = _configuration.GetConnectionString("ConexionMySql");

            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();

                var query = "SELECT id, latitud, longitud, nombre, idconductor FROM localizaciones";
                using var cmd = new MySqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    double latConductor = Convert.ToDouble(reader["latitud"]);
                    double lonConductor = Convert.ToDouble(reader["longitud"]);

                    double distancia = CalcularDistanciaHaversine((double)latitud, (double)longitud, latConductor, lonConductor);

                    if (distancia <= 10)
                    {
                        conductoresCercanos.Add(new
                        {
                            idConductor = reader["idconductor"],
                            nombre = reader["nombre"],
                            latitud = reader["latitud"],
                            longitud = reader["longitud"],
                            distancia = Math.Round(distancia, 2)
                        });
                    }
                }

                if (conductoresCercanos.Count == 0)
                    return NotFound(new { error = "No se encontraron conductores cercanos" });

                return Ok(conductoresCercanos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al procesar la solicitud", detalles = ex.Message });
            }
        }

        private double CalcularDistanciaHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double lat1Rad = ToRadians(lat1);
            double lat2Rad = ToRadians(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return RadioTierraKm * c;
        }

        private double ToRadians(double grados) => grados * Math.PI / 180;
    }






}

