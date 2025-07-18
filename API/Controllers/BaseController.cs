using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BaseController : ControllerBase
{
    protected ActionResult HandleResponse<T>(Response<T> response)
    {
        return StatusCode((int)response.Status, response);
    }
}
