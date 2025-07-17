using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BaseLocationsController : ControllerBase
{
    protected ActionResult HandleResponse<T>(Response<T> response)
    {
        return StatusCode((int)response.Status, response);
    }
}
