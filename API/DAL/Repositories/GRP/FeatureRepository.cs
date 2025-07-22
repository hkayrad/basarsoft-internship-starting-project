using System;
using API.Models;
using API.Models.Contexts;

namespace API.Repositories.GRP;

public class FeatureRepository(MapInfoContext context) : GenericRepository<Feature>(context)
{
}
