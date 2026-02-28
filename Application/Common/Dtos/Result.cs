namespace Application.Common.Dtos
{
    public class Result<T>
    {
        public string Message { get; set; } = default!;
        public T? Data { get; set; }
        public bool Status  { get; set; }

        public static Result<T> Success(string message, T data)
        {
            return new Result<T>
            {
                Message = message,
                Data = data,
                Status = true
            };
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>
            {
                Message = message
            };
        }
    }
    
}
