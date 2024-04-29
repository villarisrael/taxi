using Certificado2.Modelos;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Certificado2.Servicios
{

    public interface IRepositorioMonedas
    {
        Task<IEnumerable<Moneda>> ObtenerDatosMoneda(string serie, int folio);
        Task<byte[]> ObtenerLogotipoEmpresaAsync();
    }
    public class RepositorioMonedas : IRepositorioMonedas
    {

        private readonly string connectionString;

        public RepositorioMonedas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<byte[]> ObtenerLogotipoEmpresaAsync()
        {
            byte[] logotipo = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT Logo_Empresa FROM empresa";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                logotipo = (byte[])reader["Logo_Empresa"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el logotipo de la empresa: {ex.Message}");
            }

            return logotipo;
        }

        public async Task<IEnumerable<Moneda>> ObtenerDatosMoneda(string serie, int folio)
        {
            List<Moneda> listado = new List<Moneda>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Error en esta consulta, no tengo acceso a la BD
                    string selectQuery = $"SELECT * FROM MONEDA WHERE SERIE = '{serie}' AND FOLIO = {folio}";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Moneda certificador = new Moneda
                                {
                                    IdCertificado = (int)reader["id"],
                                    Serie = reader["RazonSocial"] as string,
                                    Folio = (int)reader["NombreResponsable"],
                                    IdCertificador = (int)reader["Email"],
                                    IdUsuario = (int)reader["CP"],
                                    Nombre = reader["Telefono"] as string,
                                    Ano = (int)reader["Emailfacturacion"],
                                    Ceca = reader["RFC"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    Foto = reader["logo"] as byte[]
                                };

                                listado.Add(certificador);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de la moneda: {ex.Message}");
            }

            return listado;
        }

    }
}
