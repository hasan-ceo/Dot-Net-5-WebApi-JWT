using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuteexy.Models
{
    [Table("UserRefreshToken", Schema = "Idt")]
    public class UserRefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserRefreshTokenID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [MaxLength(50)]
        public string RefreshToken { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime ExpiryDate { get; set; }
    }
}
