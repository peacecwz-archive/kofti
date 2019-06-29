using Kofti.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kofti.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ValuesController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet("{key}")]
        public ActionResult<string> Get(string key)
        {
            return _configService.GetValue<string>(key, $"Not available config key: '{key}'");
        }
    }
}