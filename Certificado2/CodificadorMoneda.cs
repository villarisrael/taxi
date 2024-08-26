namespace Certificado2
{
    using Certificado2.Modelos;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class CodificadorMoneda
    {
        public string CodificarMoneda(VMoneda datosMoneda)
        {
            // Concatenar los atributos de la moneda
            string atributosMoneda = $"{datosMoneda.Nombre}-{datosMoneda.Ano}-{datosMoneda.Ceca}-{datosMoneda.Material}-{datosMoneda.Estado}";

            // Agregar la palabra base
            string cadenaAProcesar = atributosMoneda + "Villar01";

            // Aplicar un algoritmo de codificación seguro (SHA256)
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesCodificados = sha256.ComputeHash(Encoding.UTF8.GetBytes(cadenaAProcesar));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytesCodificados.Length; i++)
                {
                    builder.Append(bytesCodificados[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

    public class CodificadorJoyeria
    {
        public string CodificarJoyeria(VJoyeria datosjoyeria)
        {
            // Concatenar los atributos de la moneda
            string atributosMoneda = $"{datosjoyeria.Marca}-{datosjoyeria.Objeto}-{datosjoyeria.Marca}-{datosjoyeria.Estado}";

            // Agregar la palabra base
            string cadenaAProcesar = atributosMoneda + "Villar01";

            // Aplicar un algoritmo de codificación seguro (SHA256)
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesCodificados = sha256.ComputeHash(Encoding.UTF8.GetBytes(cadenaAProcesar));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytesCodificados.Length; i++)
                {
                    builder.Append(bytesCodificados[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }


    public class CodificadorArtesania
    {
        public string CodificarArtesania(VArtesania datosartesania)
        {
            // Concatenar los atributos de la moneda
            string atributosMoneda = $"{datosartesania.Fabricante}-{datosartesania.Descripción}-{datosartesania.Dimensiones}-{datosartesania.Peso}";

            // Agregar la palabra base
            string cadenaAProcesar = atributosMoneda + "Villar01";

            // Aplicar un algoritmo de codificación seguro (SHA256)
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesCodificados = sha256.ComputeHash(Encoding.UTF8.GetBytes(cadenaAProcesar));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytesCodificados.Length; i++)
                {
                    builder.Append(bytesCodificados[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
