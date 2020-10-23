using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuteexy.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
