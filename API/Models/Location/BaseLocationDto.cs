using System;

namespace API.Models.Location;

public class BaseLocationDto
{
    public required string Name { get; set; }

    public required string Wkt { get; set; }
}
