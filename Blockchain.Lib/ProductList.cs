using System.Text;

namespace Blockchain.Lib
{
    public class ProductList : List<Product>
    {
        public ProductList()
        {
        }

        public ProductList(IEnumerable<Product> collection) : base(collection)
        {
        }

        public ProductList(int capacity) : base(capacity)
        {
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PreviousHash { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var i in this)
            {
                builder.Append(i.ToString());
            }
            return builder.ToString();
        }
    }
}