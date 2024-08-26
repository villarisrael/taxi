namespace Certificado2.Modelos
{
    using Microsoft.EntityFrameworkCore.Metadata.Conventions;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Moneda
    {
        [Key]
        public int IdCertificado { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
        public int? IdCertificador { get; set; }
        public string idusucer { get; set; }
        public string Nombre { get; set; }
        public string Ano { get; set; }
        public string Ceca { get; set; }
        public string Material { get; set; }
        public string Estado { get; set; }
        public byte[] Foto { get; set; }

        public DateTime fecha { get; set; }


    }


    public class VMoneda
    {
        [Key]
        public int idcertificado { get; set; }
        public string  RazonSocial { get; set; }
     

        public string NombreResponsable { get; set; }

        public string Telefono { get; set; }

        public string RFC { get; set; }
        public string Serie { get; set; }
        public int Folio { get; set; }
       
        public string Nombre { get; set; }
        public string Ano { get; set; }
        public string Ceca { get; set; }
        public string Material { get; set; }
        public string Estado { get; set; }

        public DateTime fecha { get; set; }
        public byte[] Foto { get; set; }
    }

}
