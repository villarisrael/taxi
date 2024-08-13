
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Certificado2.Modelos
{
 
    public class Artesania
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDArtesania { get; set; }

        [Required]
        [StringLength(255)]
        public string Fabricante { get; set; }

        [StringLength(4)]
        public string Serie { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El folio debe ser un número positivo.")]
        public int Folio { get; set; }

        [StringLength(250)] // Ajusta la longitud si es necesario
        public string Descripción { get; set; }

        [StringLength(250)] // Ajusta la longitud si es necesario
        public string Materiales { get; set; }

        [StringLength(250)]
        public string Dimensiones { get; set; }

      
        public string Peso { get; set; }

        [Required]
        public int IDCertificador { get; set; }

        [StringLength(255)]
        public byte[] Imagen { get; set; }

        [Required]
        public DateTime FechaCreación { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [StringLength(250)]
        public string Observacion { get; set; }
    }

  
        public class VArtesania
        {
            [Key]
            public int IDArtesania { get; set; }

            [Required]
            [StringLength(255)]
            public string Fabricante { get; set; }

            [StringLength(4)]
            public string Serie { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "El folio debe ser un número positivo.")]
            public int Folio { get; set; }

            [StringLength(250)]
            public string Descripción { get; set; }

            [StringLength(250)]
            public string Materiales { get; set; }

            [StringLength(250)]
            public string Dimensiones { get; set; }

           
            public string  Peso { get; set; }

            [Required]
            public int IDCertificador { get; set; }

            public byte[] Imagen { get; set; }

            [Required]
            public DateTime FechaCreación { get; set; }

            [Required]
            public DateTime Fecha { get; set; }

            [StringLength(250)]
            public string Observacion { get; set; }

            [StringLength(255)]
            public string RazonSocial { get; set; }
        }
    }



