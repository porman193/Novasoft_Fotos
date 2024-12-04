using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace CasaToro.Novasoft.Fotos.Models
{
    [Table("gen_clasif3")]
    public class BusinessUnit
    {
        [Key]
        [Column("codigo")]
        public string? code { get; set; }

        [Column("nombre")]
        public string? name { get; set; }
    }
}
