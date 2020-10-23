using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuteexy.Models
{
    [Table("UserProfile", Schema = "Hub")]
    public class UserProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid UserProfileID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(150)]
        public string FatherName { get; set; }

        [Required]
        [MaxLength(150)]
        public string MotherName { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(20)]
        public string BloodGroup { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(20)]
        public string Religion { get; set; }

        [Display(Name = "Street Address")]
        [MaxLength(100)]
        public string StreetAddress { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [Display(Name = "Postal Code")]
        [MaxLength(100)]
        public string PostalCode { get; set; }

        [Display(Name = "Country")]
        [MaxLength(100)]
        public string Country { get; set; }

        [Required]
        [MaxLength(150)]
        public string EmergencyContactName { get; set; }

        [Required]
        [MaxLength(150)]
        public string EmergencyEmail { get; set; }

        [Required]
        [MaxLength(150)]
        public string EmergencyRelation { get; set; }

        [Required]
        [MaxLength(150)]
        public string EmergencyPhoneNumber { get; set; }

        [MaxLength(150)]
        public string ImageUrl { get; set; }

        [MaxLength(150)]
        public string FullName { get; set; }

    }
}
