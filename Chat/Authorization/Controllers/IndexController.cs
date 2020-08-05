using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
            return Content("Api is upped");
        }
    }
}
