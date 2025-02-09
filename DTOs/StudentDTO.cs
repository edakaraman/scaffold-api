namespace ScaffoldDeneme.DTOs
{
    public class StudentDTO
    {
        public string Name { get; set; } = null!;
        public string SurName { get; set; } = null!;
        public DateOnly BirthDate { get; set; }
        public long NationalityId { get; set; }
        public bool IsActive { get; set; }
    }
}
