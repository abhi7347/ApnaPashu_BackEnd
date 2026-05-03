namespace APNAPASHU.DataContract.Models.Web.Buyer.FavoriteAnimal
{
    public class FavoriteAnimalFilterDto
    {
        public int UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
