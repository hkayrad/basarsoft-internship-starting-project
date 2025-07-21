using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public partial class Feature
{
    [Column(TypeName = "int4")]
    public int Id { get; set; }

    [Column(TypeName = "varchar")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "varchar")]
    public string Wkt { get; set; } = null!;
}
