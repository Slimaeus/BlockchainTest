using Blockchain.Lib;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> InsertProducts(Block<ProductList> previousBlock)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var currentBlock = new Block<ProductList>(previousBlock.Data, previousBlock.Hash, Block<ProductList>.GetTime(previousBlock.Data.CreatedDate));
            if (string.IsNullOrEmpty(nextBlockUrl))
            {
                Products = previousBlock.Data;
                Products.CreatedDate = previousBlock.Data.CreatedDate;
                Products.PreviousHash = previousBlock.Hash;
                return Ok(new SuccessResponse<string>("Insert successfully"));
            }
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/insert-products", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<string>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    Products = previousBlock.Data;
                    Products.CreatedDate = previousBlock.Data.CreatedDate;
                    Products.PreviousHash = previousBlock.Hash;
                    return Ok(content);
                }
            return Ok(content);
        }
        [HttpPost("get-products")]
        public async Task<IActionResult> GetProducts(Block<ProductList> previousBlock)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            if (Product.PreviousHash == null)
                return Ok(new ErrorResponse<ProductList>("Not hash yet"));
            if (Product == null)
                return Ok(new ErrorResponse<ProductList>("Nothing in database"));
            var currentBlock = new Block<ProductList>(Products, Products.PreviousHash, Block<ProductList>.GetTime(Products.CreatedDate));
            if (!currentBlock.IsGenesisBlock())
                if (!previousBlock.IsHashCorrect(currentBlock.PreviousHash))
                    return Ok(new ErrorResponse<Product>("Wrong hash"));
            if (string.IsNullOrEmpty(nextBlockUrl))
                return Ok(new SuccessResponse<ProductList>(Products));
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-products", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<ProductList>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    return Ok(new SuccessResponse<ProductList>(Products));
                }
            return Ok(content);
        }
        [HttpPost("insert-product")]
        public async Task<IActionResult> InsertProduct(Block<Product> previousBlock)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var currentBlock = new Block<Product>(previousBlock.Data, previousBlock.Hash, Block<Product>.GetTime(previousBlock.Data.CreatedDate));
            if (string.IsNullOrEmpty(nextBlockUrl))
            {
                Product = previousBlock.Data;
                Product.CreatedDate = previousBlock.Data.CreatedDate;
                Product.PreviousHash = previousBlock.Hash;
                return Ok(new SuccessResponse<string>("Insert successfully"));
            }
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/insert-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<string>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    Product = previousBlock.Data;
                    Product.CreatedDate = previousBlock.Data.CreatedDate;
                    Product.PreviousHash = previousBlock.Hash;
                    return Ok(content);
                }
            return Ok(content);
        }
        [HttpPost("get-product")]
        public async Task<IActionResult> GetProduct(Block<Product> previousBlock)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            if (Product.PreviousHash == null)
                return Ok(new ErrorResponse<Product>("Not hash yet"));
            if (Product == null)
                return Ok(new ErrorResponse<Product>("Nothing in database"));
            var currentBlock = new Block<Product>(Product, Product.PreviousHash, Block<Product>.GetTime(Product.CreatedDate));
            if (!currentBlock.IsGenesisBlock())
                if (!previousBlock.IsHashCorrect(currentBlock.PreviousHash))
                    return Ok(new ErrorResponse<Product>("Wrong hash"));
            if (string.IsNullOrEmpty(nextBlockUrl))
                return Ok(new SuccessResponse<Product>(Product));
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<Product>>(content);
            if (response != null)
                if (response.IsSucceded)
                {
                    return Ok(new SuccessResponse<Product>(Product));
                }
            return Ok(content);
        }
    }
}
