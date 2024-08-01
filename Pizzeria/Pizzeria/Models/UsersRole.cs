using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria.Models
{
    public class UsersRole
    {
        public int UsersId { get; set; }
        public int RolesId { get; set; }

        [ForeignKey("RolesId")]
        [InverseProperty("UsersRoles")]
        public virtual Role Role { get; set; }

        [ForeignKey("UsersId")]
        [InverseProperty("UsersRoles")]
        public virtual User User { get; set; }
    }
}