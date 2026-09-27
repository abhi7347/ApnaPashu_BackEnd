namespace APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal
{
    public class AnimalPromotionResponseModel
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AmountPaid { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AnimalPromotionUpsertModel
    {
        public int AnimalId { get; set; }
        public int PlanId { get; set; }
        public int DurationDays { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
