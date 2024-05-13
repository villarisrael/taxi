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
        public int? Ano { get; set; }
        public string Ceca { get; set; }
        public string Material { get; set; }
        public string Estado { get; set; }
        public byte[] Foto { get; set; }
    }

}
