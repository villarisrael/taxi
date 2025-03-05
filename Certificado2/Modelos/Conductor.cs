using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Certificado2.Modelos
{
    [Table("conductor")]
    public class Conductor
    {
        [Key]
        public int IDConductor { get; set; }

        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }

        [StringLength(250)]
        [Phone]
        public string Telefono { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        [StringLength(250)]
        public string Colonia { get; set; }

        [StringLength(250)]
        public string Ciudad { get; set; }

        public string IDEstado { get; set; }

        public string IDPais { get; set; }

        [StringLength(250)]
        [EmailAddress]
        public string mail { get; set; }

        [StringLength(250)]
        public string Licencia { get; set; }

        [StringLength(250)]
        public string Placa { get; set; }

        [StringLength(250)]
        public string INE { get; set; }

        public int Activo { get; set; }

        public int IDOrganizacion { get; set; }

        [StringLength(45)]
        public string Independiente { get; set; }
    }

    public class VConductor : Conductor
    {
        public string RazonSocial { get; set; }
    }
}
