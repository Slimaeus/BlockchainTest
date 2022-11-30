using Blockchain.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Blockchain.Original.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController1 : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController1(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private Product? Product { get; set; }
        static Block? currentBlock;
        [HttpPost("insert-product")]
        public async Task<IActionResult> InsertProduct(Block block)
        {
            currentBlock = block;    
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            if (string.IsNullOrEmpty(nextBlockUrl))
                return Ok("Finish");
            Product = block.Data;
            var nextBlock = new Block(currentBlock.Data, currentBlock.Hash, Block.GetTime());
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(nextBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/insert-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            
            return Ok($"{block.TimeStamp} - {content}");
        }
        [HttpPost("get-product")]
        public async Task<IActionResult> GetProduct(Block block)
        {
            string hash = (currentBlock != null) ? currentBlock.Hash : _configuration["BlockKey:hash"];
            string previousHash = (currentBlock != null) ? currentBlock.PreviousHash : _configuration["BlockKey:previousHash"];
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            if (currentBlock == null)
                return Ok("false");
            if (previousHash != "0")
                if (block.Hash != previousHash)
                    return Ok("false");
            if (string.IsNullOrEmpty(nextBlockUrl))
                return Ok(currentBlock.Data.Name);
            var client = new HttpClient();

            block.Hash = hash;
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(block), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            if (content != "false")
                return Ok(hash + " " + content);
            return Ok("false");
        }
    }
}
