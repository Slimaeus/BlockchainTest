namespace Blockchain.Lib
{
    public class Response<T>
    {
        public bool IsSucceded { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}