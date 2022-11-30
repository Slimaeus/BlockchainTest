namespace Blockchain.Lib
{
    public class ErrorResponse<T> : Response<T>
    {
        public ErrorResponse()
        {
            IsSucceded = false;
        }
        public ErrorResponse(string message)
        {
            IsSucceded = false;
            Message = message;
        }
    }
}