namespace Blockchain.Lib
{
    public class SuccessResponse<T> : Response<T>
    {
        public SuccessResponse(T value)
        {
            IsSucceded = true;
            Result = value;
        }
    }
}