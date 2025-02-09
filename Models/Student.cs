using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ScaffoldDeneme.Models;

public partial class Student
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string SurName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public long NationalityId { get; set; }

    public bool IsActive { get; set; }

    [JsonIgnore]
    public virtual Nationality Nationality { get; set; } = null!;
}
