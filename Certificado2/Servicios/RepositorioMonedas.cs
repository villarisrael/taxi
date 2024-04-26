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

    }
}
