using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
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
public class StudentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDatabase _redisDatabase;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IUnitOfWork unitOfWork, IMapper mapper, 
    IConnectionMultiplexer redis, ILogger<StudentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisDatabase = redis.GetDatabase();
        _logger = logger; 
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
    {
        var cacheKey = "students";
        var cachedData = await _redisDatabase.StringGetAsync(cacheKey);

        if (cachedData.HasValue)
        {
            _logger.LogInformation("Redis cache'inden 'students' verisi alındı.");
            var cachedString = cachedData.ToString();
            if (!string.IsNullOrEmpty(cachedString))
            {
                var students = JsonSerializer.Deserialize<IEnumerable<Student>>(cachedString);
                if (students != null)
                {
                    var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);
                    return Ok(studentDtos);
                }
            }
        }

        _logger.LogInformation("Redis cache'inde 'students' verisi bulunamadı, veritabanından alınıyor.");
        var studentsFromDb = await _unitOfWork.Students.GetAllAsync();
        var studentDtosFromDb = _mapper.Map<IEnumerable<StudentDTO>>(studentsFromDb);

        var serializedData = JsonSerializer.Serialize(studentsFromDb);
        await _redisDatabase.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10)); // 10 dakika geçerlilik süresi

        _logger.LogInformation("Veritabanından alınan 'students' verisi Redis cache'ine kaydedildi.");
        return Ok(studentDtosFromDb);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> GetActiveStudents()
    {
        var students = await _unitOfWork.Students.GetActiveStudentsAsync();
        var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);
        return Ok(studentDtos);
    }

    [HttpGet("sortedByName")]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentsSortedByName()
    {
        var students = await _unitOfWork.Students.GetStudentsSortedByNameAsync();
        var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);
        return Ok(studentDtos);
    }

    [HttpGet("distinctNames")]
    public async Task<ActionResult<IEnumerable<string>>> GetDistinctStudentNames()
    {
        var names = await _unitOfWork.Students.GetDistinctStudentNamesAsync();
        return Ok(names);
    }

    [HttpGet("byAgeOrName")]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentsByAgeOrName([FromQuery] int year, [FromQuery] string name)
    {
        var students = await _unitOfWork.Students.GetStudentsByAgeOrNameAsync(year, name);
        var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);
        return Ok(studentDtos);
    }

    [HttpGet("fullNames")]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentFullNames()
    {
        var students = await _unitOfWork.Students.GetStudentFullNamesAsync();
        var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);
        return Ok(studentDtos);
    }

    [HttpGet("orderedByName")]
    public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudentsOrderName()
    {
        var students = await _unitOfWork.Students.GetStudentsOrderNameAsync();
        var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);
        return Ok(studentDtos);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StudentDTO>> GetStudent(long id)
    {
        var cacheKey = $"student:{id}";
        var cachedData = await _redisDatabase.StringGetAsync(cacheKey);

        if (cachedData.HasValue)
        {
            _logger.LogInformation("Redis cache'inden öğrenci verisi alındı, ID: {Id}", id);
            var cachedString = cachedData.ToString();
            if (!string.IsNullOrEmpty(cachedString))
            {
                var student = JsonSerializer.Deserialize<Student>(cachedString);
                if (student != null)
                {
                    var studentDto = _mapper.Map<StudentDTO>(student);
                    return Ok(studentDto);
                }
            }
        }

        _logger.LogInformation("Redis cache'inde öğrenci verisi bulunamadı, veritabanından alınıyor, ID: {Id}", id);
        var studentFromDb = await _unitOfWork.Students.GetByIdAsync(id);
        if (studentFromDb == null)
        {
            return NotFound();
        }

        var studentDtoFromDb = _mapper.Map<StudentDTO>(studentFromDb);
        var serializedData = JsonSerializer.Serialize(studentFromDb);
        await _redisDatabase.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10));

        _logger.LogInformation("Veritabanından alınan öğrenci verisi Redis cache'ine kaydedildi, ID: {Id}", id);
        return Ok(studentDtoFromDb);
    }

    [HttpPost("studentAdd")]
    public async Task<IActionResult> PostStudent([FromBody] Student student)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Yeni öğrenci eklendi, ID: {Id}. Redis cache'ini temizleme işlemi yapılıyor.", student.Id);
            await _redisDatabase.KeyDeleteAsync("students");

            return Ok(student);
        }
        return BadRequest(ModelState);
    }
}
