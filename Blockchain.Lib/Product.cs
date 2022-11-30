namespace Blockchain.Lib
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviousHash { get; set; } = "0";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public override string ToString()
        {
            return $"{Id}-{Name}-{Description}-{PreviousHash}-{CreatedDate}";
        }
    }
}