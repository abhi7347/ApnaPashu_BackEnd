namespace APNAPASHU.DataContract.Models
{
    /// <summary>
    /// Generic JSON response model for API
    /// </summary>
    public class JsonModel<T>
    {
        public JsonModel() { }

        public JsonModel(object data, string message, int statusCode, string appError = "")
        {
            Data = data;
            Message = message;
            StatusCode = statusCode;
            AppError = appError;
        }

        public string AppError { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string AccessToken { get; set; }
    }

    /// <summary>
    /// Base pagination DTO
    /// </summary>
    public class BasePaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (TotalRecords + PageSize - 1) / PageSize;
    }

    /// <summary>
    /// Status response model
    /// </summary>
    public class StatusModel
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}