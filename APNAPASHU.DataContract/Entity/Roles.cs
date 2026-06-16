using System.ComponentModel.DataAnnotations;

namespace APNAPASHU.DataContract.Entity
{
    public class Roles : BaseEntity
    {
        [Key]
        new public int Id { get; set; }
        
        public string RoleName { get; set; } = null!;
        public string? RoleDescription { get; set; }
    }
}
