using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class RedisController : ControllerBase
{
    private readonly RedisService _redisService;

    public RedisController(RedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpGet("set")]
    public async Task<IActionResult> Set(string key, string value)
    {
        await _redisService.SetStringAsync(key, value);
        return Ok("Value set successfully");
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get(string key)
    {
        var value = await _redisService.GetStringAsync(key);
        return Ok(value);
    }
}
