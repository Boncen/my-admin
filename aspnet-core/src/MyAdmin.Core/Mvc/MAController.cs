using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAdmin.Core.Mvc;
[ApiController]
[Authorize]
public class MAController:ControllerBase
{
    
}