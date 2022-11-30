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
        [HttpPost("get-product")]
        public async Task<IActionResult> GetProduct(Block block)
        {
            string hash = _configuration["BlockKey:hash"];
            string previousHash = _configuration["BlockKey:previousHash"];
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            if (previousHash != "0")
                if (block.Hash != previousHash)
                    return Ok("false");
            if (string.IsNullOrEmpty(nextBlockUrl))
                return Ok("true");
            var client = new HttpClient();
            block.Hash = hash;
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(block), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            if (content == "true")
                return Ok(new Product() { Name = hash, Description = nextBlockUrl });
            return Ok(result);
        }
    }
}
