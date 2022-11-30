using Blockchain.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private static long TimeStamp = 0;
        private static Product? Product { get; set; }
        private static string? PreviousHash;
        //static Block? currentBlock;
        [HttpPost("insert-product")]
        public async Task<IActionResult> InsertProduct(Block block)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            PreviousHash = block.Hash;
            var nextBlock = new Block(block.Data, block.Hash, Block.GetTime());
            TimeStamp = nextBlock.TimeStamp;
            if (string.IsNullOrEmpty(nextBlockUrl))
            {
                Product = block.Data;
                return Ok("Finish");
            }
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(nextBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/insert-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            if (content != "false")
            {
                Product = block.Data;
                return Ok($"Response - ({PreviousHash}) {content}");
            }
            return Ok("false");
        }
        [HttpPost("get-product")]
        public async Task<IActionResult> GetProduct(Block block)
        {
            string previousHash = (PreviousHash != null) ? PreviousHash : "0";
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            if (previousHash != "0")
                if (block.Hash != previousHash)
                    return Ok(new ErrorResponse<Product>("Wrong hash"));
            if (Product == null)
                return Ok(new ErrorResponse<Product>("Nothing in database"));
            if (string.IsNullOrEmpty(nextBlockUrl))
                return Ok(new SuccessResponse<Product>(Product));
            var client = new HttpClient();
            var currentBlock = new Block(Product, PreviousHash, TimeStamp);
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            return Ok(content);
        }
    }
}
