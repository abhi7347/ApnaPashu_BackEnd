namespace APNAPASHU.DataContract.Models
{
    /// <summary>
    /// Generic JSON response model for API
    /// </summary>
    public class JsonModel<T>
    {

        public JsonModel() { }

        public JsonModel(object? data, string? message, int? statusCode, string? appError = "")
        {
            Data = data;
            Message = message;
            StatusCode = statusCode;
            AppError = appError;
        }

        public string? AppError { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
        public int? StatusCode { get; set; }
        public string? AccessToken { get; set; }
    }

    public class CommonAuditDto
    {
        public int Id { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int? DeletedBy { get; set; }

        public int? TotalRecords { get; set; }
    }

    public class UpdateStatusDto
    {
        public int Id { get; set; }
        public bool Status { get; set; }

        public int? UserId { get; set; }
    }

    public class EmailModel
    {
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

}
