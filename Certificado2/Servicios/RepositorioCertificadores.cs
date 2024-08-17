using Certificado2.Modelos;
using Microsoft.AspNetCore.Authentication;
using MySqlConnector;
using System.Security.Claims;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Certificado2.Servicios
{
    



    public interface IRepositorioCertificadores
    {
        Task CrearAsync(Certificadores objFuentesAbastecimiento);
        Task<IEnumerable<Certificadores>> ObtenerListado();
        Task<IEnumerable<Certificadores>> ObtenerListadoCertifica(string _Certificador);
        Task ModificarAsync(Certificadores objFuentesAbastecimiento);
        Task ModificarCLogoAsync(Certificadores objFuentesAbastecimiento);
        Task SuspenderCertificador(int id);
        Task ActivarCertificador(int id);
        Task<Certificadores> ObtenerDetalleAsync(int idCertificadores);
        Task EliminarAsync(int id);
       
        Task<IEnumerable<UsuarioCertificados>> ObtenerListadoUsuarios(int _Certificador, string Usuario);
        Task<UsuarioCertificados> ObtenerDetalleUsuario(int id);
    }

    public class RepositorioCertificadores: IRepositorioCertificadores
    {
        private readonly string connectionString;

        public RepositorioCertificadores(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        //Método ObtenerListado
        public async Task<IEnumerable<Certificadores>> ObtenerListado()
        {
            List<Certificadores> listado = new List<Certificadores>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM certificadores order by RazonSocial";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Certificadores certificador = new Certificadores
                                {
                                    Id = (int)reader["id"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Email = reader["Email"] as string,
                                    CP = reader["CP"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    EmailFacturacion = reader["Emailfacturacion"] as string,
                                    RFC = reader["RFC"] as string,
                                    Logo = reader["logo"] as byte[],
                                    Suspendido = (bool)reader["Suspendido"] ,
                                    IDVendedor = (int)reader["IDVendedor"]
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

        public async Task<IEnumerable<Certificadores>> ObtenerListadoCertifica(string _certificador)
        {
            List<Certificadores> listado = new List<Certificadores>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM certificadores where  RazonSocial like '%" + _certificador + "%' or  RazonSocial like '" + _certificador + "%' or  NombreResponsable like '%"+ _certificador+ "%'  or  NombreResponsable like '"+ _certificador+ "%' order by RazonSocial " ;

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Certificadores certificador = new Certificadores
                                {
                                    Id = (int)reader["id"],
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Email = reader["Email"] as string,
                                    CP = reader["CP"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    EmailFacturacion = reader["Emailfacturacion"] as string,
                                    RFC = reader["RFC"] as string,
                                    Logo = reader["logo"] as byte[],
                                    Suspendido = (bool)reader["Suspendido"],
                                    IDVendedor = (int)reader["IDVendedor"]
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

        public async Task CrearAsync(Certificadores objCertificadores)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string insertQuery = "INSERT INTO certificadores (`RazonSocial`, `NombreResponsable`, `Email`, `CP`, `Telefono`, `Emailfacturacion`, `RFC`, Suspendido,`logo`, IDVendedor) VALUES " +
                                         "(@RazonSocial, @NombreResponsable, @Email, @CP, @Telefono, @Emailfacturacion, @RFC,@Suspendido, @Logo)";

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
                        insertCommand.Parameters.AddWithValue("@Logo", objCertificadores.Logo);

                        insertCommand.Parameters.AddWithValue("@IDVendedor", objCertificadores.IDVendedor);

                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear certificador: {ex.Message}");
            }
        }


        public async Task ModificarCLogoAsync(Certificadores objCertificadores)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE certificadores SET `RazonSocial` = @RazonSocial, `NombreResponsable` = @NombreResponsable, `Email` = @Email, `CP` = @CP, `Telefono` = @Telefono, `Emailfacturacion` = @Emailfacturacion, `RFC` = @RFC,Suspendido=@Suspendido, IDVendedor= @IDVendedor WHERE `id` = @Id";

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
                        updateCommand.Parameters.AddWithValue("@IDVendedor", objCertificadores.IDVendedor);
                        //updateCommand.Parameters.AddWithValue("@Logo", objCertificadores.Logo);
                        updateCommand.Parameters.AddWithValue("@Id", objCertificadores.Id);

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar certificador: {ex.Message}");
            }
        }



        public async Task ModificarAsync(Certificadores objCertificadores)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE certificadores SET `RazonSocial` = @RazonSocial, `NombreResponsable` = @NombreResponsable, `Email` = @Email, `CP` = @CP, `Telefono` = @Telefono, `Emailfacturacion` = @Emailfacturacion, `RFC` = @RFC,Suspendido=@Suspendido, `logo` = @Logo, IDVendedor=@IDVendedor WHERE `id` = @Id";

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
                        updateCommand.Parameters.AddWithValue("@IDVendedor", objCertificadores.IDVendedor);
                        updateCommand.Parameters.AddWithValue("@Id", objCertificadores.Id);


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

                    string updateQuery = "UPDATE certificadores SET Suspendido = @Suspendido WHERE id = @Id";

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

        public async Task ActivarCertificador(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE certificadores SET `Suspendido` = @Suspendido WHERE `id` = @Id";

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


        public async Task EliminarAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string deleteQuery = "DELETE FROM Certificadores WHERE id = @Id";

                    using (var deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@Id", id);

                        await deleteCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar fuente: {ex.Message}");
            }
        }



        public async Task<Certificadores> ObtenerDetalleAsync(int id)
        {
            Certificadores certificadores = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM certificadores WHERE id = @Id";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@Id", id);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                certificadores = new Certificadores
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    RazonSocial = reader["RazonSocial"] as string,
                                    NombreResponsable = reader["NombreResponsable"] as string,
                                    Email = reader["Email"] as string,
                                    CP = reader["CP"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    EmailFacturacion = reader["EmailFacturacion"] as string,
                                    RFC = reader["RFC"] as string,
                                    Suspendido = (bool)reader["Suspendido"],
                                    Logo = reader["Logo"] as byte[],
                                    IDVendedor = Convert.ToInt32(reader["IDVendedor"])
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
                                    idcertificador = (int)reader["idcertificador"],
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


        public async Task<UsuarioCertificados> ObtenerDetalleUsuario(int id)
        {
            // Busca el usuario en la base de datos por su ID
            UsuarioCertificados usuario = new UsuarioCertificados();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT * FROM aspnetusers where  idcentificador=" + id;

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                 usuario = new UsuarioCertificados
                                {
                                    Id = reader["Id"] as string,
                                    idcertificador = reader["idcertificador"] != DBNull.Value ? Convert.ToInt32(reader["idcertificador"]) : 0,
                                    NombreCompleto = reader["NombreCompleto"] as string,
                                    UserName = reader["usuario"] as string,
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
    }
}
