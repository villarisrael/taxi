using Certificado2.Modelos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Security.Claims;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Certificado2.Servicios
{
    



    public interface IRepositorioOrganizaciones
    {
        Task <bool> CrearAsync(Organizacion objCertificadores  );
        Task<IEnumerable<Organizacion>> ObtenerListado();
        Task<IEnumerable<Organizacion>> ObtenerListadoCertifica(string _Certificador);

     

        Task ModificarAsync(Organizacion objFuentesAbastecimiento);
        Task ModificarCLogoAsync(Organizacion objFuentesAbastecimiento);
        Task SuspenderCertificador(int id);
        Task ActivarCertificador(int id);
        Task<Organizacion> ObtenerDetalleAsync(int idCertificadores);

        Task<Organizacion> ObtenerDetallexrazonSocialAsync(string Razon_social);

        Task AsignarOrganizacion(string idusuario, int idorganziacion);
        Task <bool> EliminarAsync(int id);
       
        Task<IEnumerable<UsuarioCertificados>> ObtenerListadoUsuarios(int _Certificador, string Usuario);
        Task<UsuarioCertificados> ObtenerDetalleUsuario(string id);

        Task<bool> ActualizarUsuario(UsuarioCertificados usuario);

        
        Task<string> GetUserIdByUsername(string username);

        Task<IdentityResult> AddClaimToUserAsync(string username, string claimType, string claimValue);
        Task<IdentityResult> RemoveClaimFromUserAsync(string username, string claimType, string claimValue);  // Nuevo método para eliminar un claim
        Task<IdentityResult> UpdateClaimForUserAsync(string username, string claimType, string newClaimValue);  // Nuevo método para modificar un claim

    }

    public class RepositorioOrganizaciones : IRepositorioOrganizaciones
    {
        private readonly string connectionString;
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly SignInManager<UsuarioCertificados> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public RepositorioOrganizaciones(IConfiguration configuration, UserManager<UsuarioCertificados> userManager,
                                 SignInManager<UsuarioCertificados> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }
        
        //Método ObtenerListado
        public async Task<IEnumerable<Organizacion>> ObtenerListado()
        {
            List<Organizacion> listado = new List<Organizacion>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM organizacion order by RazonSocial";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Organizacion certificador = new Organizacion
                                {
                                    id = (int)reader["id"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Email = reader["Email"] as string,
                                    CP = reader["CP"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    EmailFacturacion = reader["Emailfacturacion"] as string,
                                    RFC = reader["RFC"] as string,
                                    Logo = reader["logo"] as byte[],
                                    Suspendido = (bool)reader["Suspendido"] ,
                                 
                                    
                                };

                                listado.Add(certificador);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de certificadores: {ex.Message}");
            }

            return listado;
        }

        public async Task<IEnumerable<Organizacion>> ObtenerListadoCertifica(string _certificador)
        {
            List<Organizacion> listado = new List<Organizacion>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM organizacion where  RazonSocial like '%" + _certificador + "%' or  RazonSocial like '" + _certificador + "%' or  NombreResponsable like '%"+ _certificador+ "%'  or  NombreResponsable like '"+ _certificador+ "%' order by RazonSocial " ;

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Organizacion certificador = new Organizacion
                                {
                                    id = (int)reader["id"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Email = reader["Email"] as string,
                                    CP = reader["CP"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    EmailFacturacion = reader["Emailfacturacion"] as string,
                                    RFC = reader["RFC"] as string,
                                    Logo = reader["logo"] as byte[],
                                    Suspendido = (bool)reader["Suspendido"]
                               
                                };

                                listado.Add(certificador);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de certificadores: {ex.Message}");
            }

            return listado;
        }


        public async Task <bool> CrearAsync(Organizacion objCertificadores)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string insertQuery = @"
            INSERT INTO organizacion 
            (`RazonSocial`, `NombreResponsable`, `Email`, `CP`, `Telefono`, `Emailfacturacion`, `RFC`, `Suspendido`, `logo`)    VALUES    (@RazonSocial, @NombreResponsable, @Email, @CP, @Telefono, @Emailfacturacion, @RFC, @Suspendido, @Logo)";

                    using (var insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@RazonSocial", objCertificadores.RazonSocial);
                        insertCommand.Parameters.AddWithValue("@NombreResponsable", objCertificadores.NombreResponsable);
                        insertCommand.Parameters.AddWithValue("@Email", objCertificadores.Email);
                        insertCommand.Parameters.AddWithValue("@CP", objCertificadores.CP);
                        insertCommand.Parameters.AddWithValue("@Telefono", objCertificadores.Telefono);
                        insertCommand.Parameters.AddWithValue("@Emailfacturacion", objCertificadores.EmailFacturacion);
                        insertCommand.Parameters.AddWithValue("@RFC", objCertificadores.RFC);
                        insertCommand.Parameters.AddWithValue("@Suspendido", objCertificadores.Suspendido);
                        insertCommand.Parameters.AddWithValue("@Logo", objCertificadores.Logo ?? (object)DBNull.Value);
                       

                        int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear certificador: {ex.Message}");
                return false;
            }
        }




        public async Task ModificarCLogoAsync(Organizacion objCertificadores)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE organizacion SET `RazonSocial` = @RazonSocial, `NombreResponsable` = @NombreResponsable, `Email` = @Email, `CP` = @CP, `Telefono` = @Telefono, `Emailfacturacion` = @Emailfacturacion," +
                        " `RFC` = @RFC,Suspendido=@Suspendido  WHERE `id` = @Id";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@RazonSocial", objCertificadores.RazonSocial);
                        updateCommand.Parameters.AddWithValue("@NombreResponsable", objCertificadores.NombreResponsable);
                        updateCommand.Parameters.AddWithValue("@Email", objCertificadores.Email);
                        updateCommand.Parameters.AddWithValue("@CP", objCertificadores.CP);
                        updateCommand.Parameters.AddWithValue("@Telefono", objCertificadores.Telefono);
                        updateCommand.Parameters.AddWithValue("@Emailfacturacion", objCertificadores.EmailFacturacion);
                        updateCommand.Parameters.AddWithValue("@RFC", objCertificadores.RFC);
                        updateCommand.Parameters.AddWithValue("@suspendido", objCertificadores.Suspendido);
                   
                        //updateCommand.Parameters.AddWithValue("@Logo", objCertificadores.Logo);
                        updateCommand.Parameters.AddWithValue("@Id", objCertificadores.id);

                       
                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar certificador: {ex.Message}");
            }
        }



        public async Task ModificarAsync(Organizacion objCertificadores)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE organizacion SET `RazonSocial` = @RazonSocial, `NombreResponsable` = @NombreResponsable, `Email` = @Email, `CP` = @CP, `Telefono` = @Telefono, " +
                        "`Emailfacturacion` = @Emailfacturacion, `RFC` = @RFC,Suspendido=@Suspendido, `logo` = @Logo   WHERE `id` = @id";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@RazonSocial", objCertificadores.RazonSocial);
                        updateCommand.Parameters.AddWithValue("@NombreResponsable", objCertificadores.NombreResponsable);
                        updateCommand.Parameters.AddWithValue("@Email", objCertificadores.Email);
                        updateCommand.Parameters.AddWithValue("@CP", objCertificadores.CP);
                        updateCommand.Parameters.AddWithValue("@Telefono", objCertificadores.Telefono);
                        updateCommand.Parameters.AddWithValue("@Emailfacturacion", objCertificadores.EmailFacturacion);
                        updateCommand.Parameters.AddWithValue("@RFC", objCertificadores.RFC);
                        updateCommand.Parameters.AddWithValue("@suspendido", objCertificadores.Suspendido);
                        updateCommand.Parameters.AddWithValue("@Logo", objCertificadores.Logo);
                       
                        updateCommand.Parameters.AddWithValue("@id", objCertificadores.id);

                        
                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar certificador: {ex.Message}");
            }
        }
        
        public async Task SuspenderCertificador(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE organizacion SET Suspendido = @Suspendido WHERE id = @Id";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        
                        updateCommand.Parameters.AddWithValue("@Suspendido", true);
                       
                        updateCommand.Parameters.AddWithValue("@Id", id);

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al suspender certificador: {ex.Message}");
            }
        }


        public async Task AsignarOrganizacion(string idusuario, int idorganziacion)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE aspnetusers SET idorganizacion = @idorganizacion WHERE id = @Id";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {

                        updateCommand.Parameters.AddWithValue("@idorganizacion", idorganziacion);

                        updateCommand.Parameters.AddWithValue("@Id", idusuario);

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asignar idorganizacion a un usuario: {ex.Message}");
            }
        }






        public async Task ActivarCertificador(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE organizacion SET `Suspendido` = @Suspendido WHERE `id` = @Id";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {

                        updateCommand.Parameters.AddWithValue("@Suspendido", false);

                        updateCommand.Parameters.AddWithValue("@Id", id);

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al suspender certificador: {ex.Message}");
            }
        }


        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string deleteQuery = "DELETE FROM organizacion WHERE id = @Id";

                    using (var deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                      
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la organización: {ex.Message}");
               
                return false;
            }
        }




        public async Task<Organizacion> ObtenerDetalleAsync(int id)
        {
            Organizacion certificadores = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM organizacion WHERE id = @Id";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@Id", id);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                certificadores = new Organizacion
                                {
                                    id = Convert.ToInt32(reader["id"]),
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Email = reader["Email"] as string,
                                    CP = reader["CP"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    EmailFacturacion = reader["EmailFacturacion"] as string,
                                    RFC = reader["RFC"] as string,
                                    Suspendido = (bool)reader["Suspendido"],
                                    Logo = reader["Logo"] as byte[]
                                   
                                 
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalle de certificador: {ex.Message}");
            }

            return certificadores;
        }

       



        public async Task<Organizacion> ObtenerDetallexrazonSocialAsync(string razon_social)
        {
            Organizacion certificadores = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM organizacion WHERE  RazonSocial = @RazonSocial";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@RazonSocial", razon_social);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                certificadores = new Organizacion
                                {
                                    id = Convert.ToInt32(reader["id"]),
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Email = reader["Email"] as string,
                                    CP = reader["CP"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    EmailFacturacion = reader["EmailFacturacion"] as string,
                                    RFC = reader["RFC"] as string,
                                    Suspendido = (bool)reader["Suspendido"],
                                    Logo = reader["Logo"] as byte[]


                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalle de certificador: {ex.Message}");
            }

            return certificadores;
        }


        public async Task<IEnumerable<UsuarioCertificados>> ObtenerListadoUsuarios(int _certificador, string U)
        {
            List<UsuarioCertificados> listado = new List<UsuarioCertificados>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM aspnetusers where  idcertificador=" + _certificador;
                    if (U!="")
                    {
                        selectQuery = "SELECT * FROM UsuCertificadores where  idcertificador=" + _certificador + " and NombreCompleto like '%"+U+"%'";
                    }
                   
                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                UsuarioCertificados certificador = new UsuarioCertificados
                                {
                                    Id = reader["Id"] as string,
                                    idOrganizacion = (int)reader["idOrganizacion"],
                                    NombreCompleto = reader["NombreCompleto"] as string,
                                    UserName = reader["UserName"] as string,
                                  
                                    Email = reader["Email"] as string,
                                    PhoneNumber = reader["PhoneNumber"] as string,
                                 

                                };

                                listado.Add(certificador);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de usuarios: {ex.Message}");
            }

            return listado;
        }


        public async Task<UsuarioCertificados> ObtenerDetalleUsuario(string id)
        {
            // Busca el usuario en la base de datos por su ID
            UsuarioCertificados usuario = new UsuarioCertificados();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM aspnetusers where  id='" + id +"'";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                 usuario = new UsuarioCertificados
                                {
                                    Id = reader["Id"] as string,
                                     idOrganizacion = reader["idOrganizacion"] != DBNull.Value ? Convert.ToInt32(reader["idcertificador"]) : 0,
                                    NombreCompleto = reader["NombreCompleto"] as string,
                                    UserName = reader["UserName"] as string,
                                    NormalizedUserName = reader["NormalizedUserName"] as string,
                                    Email = reader["Email"] as string,
                                    EmailConfirmed = reader["EmailConfirmed"] != DBNull.Value ? Convert.ToBoolean(reader["EmailConfirmed"]) : false,
                                    PhoneNumber = reader["PhoneNumber"] as string,
                                    PhoneNumberConfirmed = reader["PhoneNumberConfirmed"] != DBNull.Value ? Convert.ToBoolean(reader["PhoneNumberConfirmed"]) : false,
                                    TwoFactorEnabled = reader["TwoFactorEnabled"] != DBNull.Value ? Convert.ToBoolean(reader["TwoFactorEnabled"]) : false
                                };



                            }
                        }
                    }
                    return usuario;
                }
            }
            catch (Exception ex) { return usuario; }
        }



        public async Task<bool> ActualizarUsuario(UsuarioCertificados usuario)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = @"
                UPDATE aspnetusers 
                SET 
                    idOrganizacion = @idOrganizacion, 
                    NombreCompleto = @NombreCompleto, 
                    UserName = @UserName, 
                    NormalizedUserName = @NormalizedUserName, 
                    Email = @Email, 
                    EmailConfirmed = @EmailConfirmed, 
                    PhoneNumber = @PhoneNumber, 
                    PhoneNumberConfirmed = @PhoneNumberConfirmed, 
                    TwoFactorEnabled = @TwoFactorEnabled
                WHERE Id = @Id";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        // Agregar parámetros a la consulta
                        updateCommand.Parameters.AddWithValue("@Id", usuario.Id);
                        updateCommand.Parameters.AddWithValue("@idcertificador", usuario.idOrganizacion);
                        updateCommand.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@UserName", usuario.UserName ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@NormalizedUserName", usuario.NormalizedUserName ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Email", usuario.Email ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@EmailConfirmed", usuario.EmailConfirmed);
                        updateCommand.Parameters.AddWithValue("@PhoneNumber", usuario.PhoneNumber ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@PhoneNumberConfirmed", usuario.PhoneNumberConfirmed);
                        updateCommand.Parameters.AddWithValue("@TwoFactorEnabled", usuario.TwoFactorEnabled);

                        // Ejecutar el comando de actualización
                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                        // Verificar si se afectó alguna fila
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción si es necesario
                return false;
            }
        }
        public async Task<IdentityResult> AddClaimToUserAsync(string username, string claimType, string claimValue)
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

            var newClaim = new Claim(claimType, claimValue);

            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == claimType && c.Value == claimValue))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ClaimExists",
                    Description = $"El usuario ya tiene el claim '{claimType}' con valor '{claimValue}'."
                });
            }

            return await _userManager.AddClaimAsync(user, newClaim);
        }

        // Método para eliminar un claim
        public async Task<IdentityResult> RemoveClaimFromUserAsync(string username, string claimType, string claimValue)
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

            var claimToRemove = new Claim(claimType, claimValue);

            // Eliminar el claim
            var result = await _userManager.RemoveClaimAsync(user, claimToRemove);
            return result;
        }

        // Método para modificar un claim (eliminar el anterior y agregar uno nuevo)
        public async Task<IdentityResult> UpdateClaimForUserAsync(string iduser, string claimType, string newClaimValue)
        {
            var user = await _userManager.FindByIdAsync(iduser);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"El usuario '{iduser}' no fue encontrado."
                });
            }

            // Obtener los claims actuales del usuario
            var claims = await _userManager.GetClaimsAsync(user);

            // Buscar el claim que queremos modificar
            var claimToUpdate = claims.FirstOrDefault(c => c.Type == claimType);
            if (claimToUpdate == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ClaimNotFound",
                    Description = $"El claim '{claimType}' no fue encontrado para el usuario."
                });
            }

            // Eliminar el claim anterior
            var removeResult = await _userManager.RemoveClaimAsync(user, claimToUpdate);
            if (!removeResult.Succeeded)
            {
                return removeResult;
            }

            // Añadir el nuevo claim con el valor actualizado
            var newClaim = new Claim(claimType, newClaimValue);
            var addResult = await _userManager.AddClaimAsync(user, newClaim);
            return addResult;
        }

        public async Task<string> GetUserIdByUsername(string username)
        {
            string userId = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync(); 

                string query = "SELECT Id FROM aspnetusers WHERE UserName = @UserName LIMIT 1";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", username);

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync()) 
                    {
                        if (await reader.ReadAsync()) 
                        {
                            userId = reader["Id"].ToString();
                        }
                    }
                }
            }

            return userId; 
        }


    }
}
