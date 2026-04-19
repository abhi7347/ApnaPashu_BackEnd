using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APNAPASHU.DataContract.Entity.Seller
{
    [Table("AnimalPromotions")]
    public class AnimalPromotion : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int AnimalId { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal AmountPaid { get; set; }
    }
}
