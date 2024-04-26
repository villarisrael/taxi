using Certificado2.Modelos;
using MySqlConnector;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Certificado2.Servicios
{
   



    public interface IRepositorioCertificadores
    {
        Task CrearAsync(Certificadores objFuentesAbastecimiento);
        Task<IEnumerable<Certificadores>> ObtenerListado();
        Task ModificarAsync(Certificadores objFuentesAbastecimiento);
        Task<Certificadores> ObtenerDetalleAsync(int idCertificadores);
        Task EliminarAsync(int id);

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

                    string selectQuery = "SELECT * FROM certificadores";

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
                                    Logo = reader["logo"] as byte[]
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

                    string insertQuery = "INSERT INTO certificadores (`RazonSocial`, `NombreResponsable`, `Email`, `CP`, `Telefono`, `Emailfacturacion`, `RFC`, `logo`) VALUES " +
                                         "(@RazonSocial, @NombreResponsable, @Email, @CP, @Telefono, @Emailfacturacion, @RFC, @Logo)";

                    using (var insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@RazonSocial", objCertificadores.RazonSocial);
                        insertCommand.Parameters.AddWithValue("@NombreResponsable", objCertificadores.NombreResponsable);
                        insertCommand.Parameters.AddWithValue("@Email", objCertificadores.Email);
                        insertCommand.Parameters.AddWithValue("@CP", objCertificadores.CP);
                        insertCommand.Parameters.AddWithValue("@Telefono", objCertificadores.Telefono);
                        insertCommand.Parameters.AddWithValue("@Emailfacturacion", objCertificadores.EmailFacturacion);
                        insertCommand.Parameters.AddWithValue("@RFC", objCertificadores.RFC);
                        insertCommand.Parameters.AddWithValue("@Logo", objCertificadores.Logo);

                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear certificador: {ex.Message}");
            }
        }


        public async Task ModificarAsync(Certificadores objCertificadores)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string updateQuery = "UPDATE certificadores SET `RazonSocial` = @RazonSocial, `NombreResponsable` = @NombreResponsable, `Email` = @Email, `CP` = @CP, `Telefono` = @Telefono, `Emailfacturacion` = @Emailfacturacion, `RFC` = @RFC, `logo` = @Logo WHERE `id` = @Id";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@RazonSocial", objCertificadores.RazonSocial);
                        updateCommand.Parameters.AddWithValue("@NombreResponsable", objCertificadores.NombreResponsable);
                        updateCommand.Parameters.AddWithValue("@Email", objCertificadores.Email);
                        updateCommand.Parameters.AddWithValue("@CP", objCertificadores.CP);
                        updateCommand.Parameters.AddWithValue("@Telefono", objCertificadores.Telefono);
                        updateCommand.Parameters.AddWithValue("@Emailfacturacion", objCertificadores.EmailFacturacion);
                        updateCommand.Parameters.AddWithValue("@RFC", objCertificadores.RFC);
                        updateCommand.Parameters.AddWithValue("@Logo", objCertificadores.Logo);
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




    }
}
