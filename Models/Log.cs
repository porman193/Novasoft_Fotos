using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaToro.Novasoft.Fotos.Models
{
    [Table("NovasoftFotos_LogDescargas")]
    public class Log
    {
        [Key]
        [Column("id_log")]
        public int idLog { get; set; }
        [Required]
        [Column("id_usr")]
        public string? idUser { get; set; }

        [Required]
        [Column("nombre_usr")]
        public string? nameUser { get; set; }

        [Required]
        [Column("id_emple")]
        public string? idEmployee { get; set; }

        [Required]
        [Column("nombre_emple")]
        public string? nameEmployee { get; set; }

        [Required]
        [Column("fecha_desc")]
        public DateTime? downloadDate { get; set; }
    }
}
