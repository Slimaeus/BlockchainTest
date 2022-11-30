using Blockchain.Lib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Blockchain.Original.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static Product Product { get; set; } = new Product();
        private static ProductList Products { get; set; } = new ProductList();
        [HttpGet("get-products-hash")]
        public IActionResult GetProductsHash()
        {
            var block = new Block<ProductList>(Products, Products.PreviousHash, Block<ProductList>.GetTime(Products.CreatedDate));
            return Ok($"Current Hash: {block.Hash} - Previous Hash: {block.PreviousHash}");
        }
        [HttpPost("insert-products")]
        public async Task<IActionResult> InsertProducts(ProductList products)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var currentBlock = new Block<ProductList>(products, products.PreviousHash, Block<List<Product>>.GetTime(products.CreatedDate));
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/insert-products", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<string>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    Products = products;
                    Products.CreatedDate = products.CreatedDate;
                    Products.PreviousHash = products.PreviousHash;
                    return Ok(response);
                }
            return Ok(response);
        }
        [HttpPost("get-products")]
        public async Task<IActionResult> GetProducts()
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var client = new HttpClient();
            var currentBlock = new Block<ProductList>(Products, Products.PreviousHash, Block<ProductList>.GetTime(Products.CreatedDate));
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-products", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<ProductList>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    return Ok(new SuccessResponse<ProductList>(Products));
                }
            return Ok(response);
        }
        [HttpPost("insert-product")]
        public async Task<IActionResult> InsertProduct(Product product)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var currentBlock = new Block<Product>(product, product.PreviousHash, Block<Product>.GetTime(product.CreatedDate));
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/insert-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<string>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    Product = currentBlock.Data;
                    Product.CreatedDate = product.CreatedDate;
                    Product.PreviousHash = product.PreviousHash;
                    return Ok(response);
                }
            return Ok(response);
        }
        [HttpPost("get-product")]
        public async Task<IActionResult> GetProduct()
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var client = new HttpClient();
            var currentBlock = new Block<Product>(Product, Product.PreviousHash, Block<Product>.GetTime(Product.CreatedDate));
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<Product>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    return Ok(new SuccessResponse<Product>(Product));
                }
            return Ok(response);
        }
    }
}
