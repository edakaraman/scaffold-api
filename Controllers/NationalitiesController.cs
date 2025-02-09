using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ScaffoldDeneme.DTOs;
using ScaffoldDeneme.Models;
using ScaffoldDeneme.Repositories;
using System.Text.Json;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class NationalitiesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDatabase _redisDatabase; // Redis'in veritabanı ile etkileşimde bulunmak için
    private readonly ILogger<NationalitiesController> _logger;

    public NationalitiesController(IUnitOfWork unitOfWork, IMapper mapper, 
    IConnectionMultiplexer redis, ILogger<NationalitiesController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisDatabase = redis.GetDatabase();
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NationalityDTO>>> GetNationalities()
    {
        var cacheKey = "nationalities";
        var cachedData = await _redisDatabase.StringGetAsync(cacheKey);

        if (cachedData.HasValue)
        {
            _logger.LogInformation("Redis cache'inden 'nationalities' verisi alındı.");
            var cachedString = cachedData.ToString();
            if (!string.IsNullOrEmpty(cachedString))
            {
                var nationalities = JsonSerializer.Deserialize<IEnumerable<Nationality>>(cachedString);
                if (nationalities != null)
                {
                    var nationalityDtos = _mapper.Map<IEnumerable<NationalityDTO>>(nationalities);
                    return Ok(nationalityDtos);
                }
            }
        }
        _logger.LogInformation("Redis cache'de nationalities verisi bulunamadı..Veriler dbden çekiliyor.");

        var nationalitiesFromDb = await _unitOfWork.Nationalities.GetAllAsync();
        var nationalityDtosFromDb = _mapper.Map<IEnumerable<NationalityDTO>>(nationalitiesFromDb);

        var serializedData = JsonSerializer.Serialize(nationalitiesFromDb);
        await _redisDatabase.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10)); 

        _logger.LogInformation("Db'den çekilen nationalities verisi redis cache'ine kaydedildi.");

        return Ok(nationalityDtosFromDb);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<NationalityDTO>> GetNationality(long id)
    {
        var cacheKey = $"nationality:{id}";
        var cachedData = await _redisDatabase.StringGetAsync(cacheKey);

        if (cachedData.HasValue)
        {
            _logger.LogInformation("Redis cache'inden nationality verisi alındı, ID: {Id}", id);
            var cachedString = cachedData.ToString();
            if (!string.IsNullOrEmpty(cachedString))
            {
                var nationality = JsonSerializer.Deserialize<Nationality>(cachedString);
                if (nationality != null)
                {
                    var nationalityDto = _mapper.Map<NationalityDTO>(nationality);
                    return Ok(nationalityDto);
                }
            }
        }
        _logger.LogInformation("Redis cache'inden nationality verisi alınamadı.Dbden veri çekiliyor..");

        var nationalityFromDb = await _unitOfWork.Nationalities.GetByIdAsync(id);
        if (nationalityFromDb == null)
        {
            return NotFound();
        }

        var nationalityDtoFromDb = _mapper.Map<NationalityDTO>(nationalityFromDb);
        var serializedData = JsonSerializer.Serialize(nationalityFromDb);
        await _redisDatabase.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10)); 
        
        _logger.LogInformation("Veritabanından alınan nationality verisi Redis cache'ine kaydedildi, ID: {Id}", id);
        return Ok(nationalityDtoFromDb);
    }

    [HttpPost("countryAdd")]
    public async Task<IActionResult> PostNationality([FromBody] Nationality nationality)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Nationalities.AddAsync(nationality);
            await _unitOfWork.CompleteAsync();

            await _redisDatabase.KeyDeleteAsync("nationalities");

            return Ok(nationality);
        }
        return BadRequest(ModelState);
    }

    [HttpGet("ordered")]
    public async Task<ActionResult<IEnumerable<NationalityDTO>>> GetNationalitiesOrdered()
    {
        var nationalities = await _unitOfWork.Nationalities.GetNationalitiesOrderedAsync();
        var nationalityDtos = _mapper.Map<IEnumerable<NationalityDTO>>(nationalities);
        return Ok(nationalityDtos);
    }

    [HttpGet("byLength")]
    public async Task<ActionResult<IEnumerable<object>>> GetNationalitiesByLength()
    {
        var nationalities = await _unitOfWork.Nationalities.GetNationalitiesByLengthAsync();
        return Ok(nationalities);
    }
}