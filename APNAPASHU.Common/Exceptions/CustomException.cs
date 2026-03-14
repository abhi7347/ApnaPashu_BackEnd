namespace APNAPASHU.Common.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }

        public CustomException(string message, int statusCode = 500, string errorCode = "INTERNAL_ERROR") 
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}