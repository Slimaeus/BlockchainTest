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
        [HttpPost("insert-product")]
        public async Task<IActionResult> InsertProduct(Product product)
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var nextBlock = new Block(product, "0", Block.GetTime());
            nextBlock.Hash = "0";
            var client = new HttpClient();
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(nextBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/insert-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            if (content != "false")
            {
                return Ok($"Insert Successfully {content}");
            }
            return Ok($"Insert Product Failed {content}");
        }
        [HttpPost("get-product")]
        public async Task<IActionResult> GetProduct()
        {
            string nextBlockUrl = _configuration["BlockKey:nextBlockUrl"];
            var client = new HttpClient();
            var currentBlock = new Block(null, "0", 0);
            var stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(currentBlock), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(nextBlockUrl + "/api/Product/get-product", stringContent);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response<Product>>(content);
            return Ok(response);
        }
    }
}
