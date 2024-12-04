using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaToro.Novasoft.Fotos.Models
{
    [Table("rhh_emplea")]
    public class Employee
    {
        [Key]
        [Column ("cod_emp")]
        public  string? idEmployee { get; set; }

        [Column ("ap1_emp")]
        public string? surname1Employee { get; set; }

        [Column("ap2_emp")]
        public  string? surname2Employee { get; set; }

        [Column("nom_emp")]
        public string? nameEmployee { get; set; }

        [Column("fec_ing")]
        public DateTime? entryDate { get; set; }

        [Column("fec_egr")]
        public DateTime? dischargeDate { get; set; }

        [Column("fto_emp")]
        public byte[]? photoEmployee { get; set; }

        [Column("cod_cl3")]
        public string? codeBusinessUnit { get; set; }


    }

}
