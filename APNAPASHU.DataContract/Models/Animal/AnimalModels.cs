namespace APNAPASHU.DataContract.Models.Animal
{
    /// <summary>
    /// Upsert Animal Request Model (for both create and update)
    /// </summary>
    public class AnimalUpsertDto
    {
        public int? AnimalId { get; set; } // null for create, populated for update
        public string AnimalName { get; set; }
        public string Breed { get; set; }
        public string Category { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
    }

    /// <summary>
    /// Update Animal Status Request Model
    /// </summary>
    public class AnimalStatusUpdateDto
    {
        public int AnimalId { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Animal Filter Model for GetAll
    /// </summary>
    public class AnimalFilterDto
    {
        public string? Category { get; set; }
        public string? Location { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; } = true; // Filter only active by default
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Animal Response Model
    /// </summary>
    public class AnimalResponseDto
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; }
        public string Breed { get; set; }
        public string Category { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Animal Listing Response Model with Pagination
    /// </summary>
    public class AnimalListResponseDto
    {
        public List<AnimalResponseDto> Data { get; set; } = new List<AnimalResponseDto>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}