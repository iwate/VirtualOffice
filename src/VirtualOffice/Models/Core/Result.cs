namespace VirtualOffice.Models
{
    public partial class VirtualOfficeHub
    {
        public class Result
        {
            public bool Success { get; set; }
            public string ErrorCode { get; set; }
            public static Result CreateSucceeded() => new Result { Success = true };
            public static Result CreateFaild(string errorCode) => new Result { Success = false, ErrorCode = errorCode };
        }
    }
}
