using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreExperements.Core.Entity
{
    [Table("large_table")]
    public class LargeEntity
    {
        [Column("id", TypeName = "numeric(38, 0)")]
        public decimal Id { get; set; }

        [Column("int_value")]
        public int IntValue { get; set; }

        [Column("string_25")]
        [StringLength(25)]
        public string String25 { get; set; }

        [Column("string_100")]
        [StringLength(100)]
        public string String100 { get; set; }
    }
}