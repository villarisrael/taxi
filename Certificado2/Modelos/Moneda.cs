namespace Certificado2.Modelos
{
    using System;

    public class Moneda
    {
        public int IdCertificado { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
        public int? IdCertificador { get; set; }
        public int? IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Ano { get; set; }
        public string Ceca { get; set; }
        public string Material { get; set; }
        public string Estado { get; set; }
        public byte[] Foto { get; set; }
    }


    public class VMoneda
    {
        public string  RazonSocial { get; set; }
     

        public string NombreResponsable { get; set; }

        public string Telefono { get; set; }

        public string RFC { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
       
        public string Moneda { get; set; }
        public string Ano { get; set; }
        public string Ceca { get; set; }
        public string Material { get; set; }
        public string Estado { get; set; }

        public DateTime fecha { get; set; }
        public byte[] Foto { get; set; }
    }

}
