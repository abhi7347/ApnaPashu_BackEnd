using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APNAPASHU.DataContract.Entity.Admin
{
    [Table("Users")]
    public class Users : BaseEntity
    {
        [Key]
        [Column("UserId")]
        new public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public int? RoleId { get; set; }
        public string? PasswordHash { get; set; }
    }
}
