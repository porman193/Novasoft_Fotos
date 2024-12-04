using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaToro.Novasoft.Fotos.Models
{
    [Table("NovasoftFotos_RegistroCarnet")]
    public class Card
    {
        [Required]
        [Key]
        [Column("id_emple")]
        public string? idEmployee { get; set; }
        [Required]
        [Column("nombre_emple")]
        public string? nameEmployee { get; set; }

        [Column("fecha_entrega")]
        public DateTime? deliveryDate { get; set; }

        [Required]
        [Column("fecha_registro")]
        public DateTime? registrationDate { get; set; }
    }
}
