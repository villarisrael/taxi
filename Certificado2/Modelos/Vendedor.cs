using NuGet.Protocol.Plugins;
namespace Certificado2.Modelos
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("vendedor")]
    public class Vendedor
    {
        [Key]
        public int IDVendedor { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(15)]
        [Phone]
        public string Telefono { get; set; }
    }
}
