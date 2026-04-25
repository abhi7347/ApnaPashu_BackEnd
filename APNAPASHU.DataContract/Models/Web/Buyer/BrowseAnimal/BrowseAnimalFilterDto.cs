namespace APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal
{
    public class BrowseAnimalFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string? SearchTerm { get; set; }
        public string? Location { get; set; }
        public List<int>? CategoryIds { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string>? AgeRanges { get; set; } // e.g. ["0-12", "12-36", "36-60", "60-999"]
        public List<int>? Genders { get; set; } // e.g. [1, 2]
        public string SortBy { get; set; } = "Newest"; // 'Newest', 'PriceLowHigh', 'PriceHighLow'
    }
}
