namespace Certificado2.Modelos
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("joyeria")]
    public class Joyeria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdJoyeria { get; set; }

        [StringLength(3)]
        public string Serie { get; set; }

        public int? Folio { get; set; }

        [Required]
        [StringLength(250)]
        public string Objeto { get; set; }

        [Required]
        [StringLength(250)]
        public string Material { get; set; }

        [StringLength(100)]
        public string Estado { get; set; }

        [StringLength(150)]
        public string Marca { get; set; }
    }

}
