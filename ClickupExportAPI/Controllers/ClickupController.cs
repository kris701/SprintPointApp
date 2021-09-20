using ClickupInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClickupExportAPI.Controllers
{
    [ApiController]
    public class ClickupController : Controller
    {
        private IConfiguration _configuration;
        private IClickupInterface _clickup;

        public ClickupController(IConfiguration configuration)
        {
            _configuration = configuration;
            _clickup = new ClickupInterface.ClickupInterface(_configuration["token"], "https://api.clickup.com/api/v2");
        }

        [HttpGet("export")]
        public async Task<IActionResult> GetClickupData()
        {
            return Ok(await _clickup.GetAndBindAllTasks());
        }
    }
}
