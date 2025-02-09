using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ScaffoldDeneme.Models;

public partial class Nationality
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;
    
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
