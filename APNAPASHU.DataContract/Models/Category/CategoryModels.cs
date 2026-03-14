namespace APNAPASHU.DataContract.Models.Category
{
    /// <summary>
    /// Upsert Category Request Model (for both create and update)
    /// </summary>
    public class CategoryUpsertDto
    {
        public int? CategoryId { get; set; } // null for create, populated for update
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Update Category Status Request Model
    /// </summary>
    public class CategoryStatusUpdateDto
    {
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Category Filter Model for GetAll
    /// </summary>
    public class CategoryFilterDto
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; } = true; // Filter only active by default
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Category Response Model
    /// </summary>
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Category List Response Model with Pagination
    /// </summary>
    public class CategoryListResponseDto
    {
        public List<CategoryResponseDto> Data { get; set; } = new List<CategoryResponseDto>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
