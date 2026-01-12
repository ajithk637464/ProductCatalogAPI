using Microsoft.AspNetCore.Mvc;
using ProductCatalogAPI.Interfaces;

namespace ProductCatalogAPI.Controllers
{
    [ApiController]
    [Route("api/cache")]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpPost("{key}")]
        public async Task<IActionResult> Set(string key, [FromBody] string value)
        {
            await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(5));
            return Ok("Value cached");
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var value = await _cacheService.GetAsync<string>(key);
            return value == null ? NotFound() : Ok(value);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Remove(string key)
        {
            await _cacheService.RemoveAsync(key);
            return Ok("Key removed");
        }

        [HttpGet("exists/{key}")]
        public async Task<IActionResult> Exists(string key)
        {
            return Ok(await _cacheService.ExistsAsync(key));
        }

        [HttpGet("keys")]
        public async Task<IActionResult> Keys()
        {
            return Ok(await _cacheService.GetKeysAsync());
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            await _cacheService.ClearAllAsync();
            return Ok("Cache cleared");
        }
    }
}
