using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

    public class WeatherForecastController : ControllerBase
    {
        private readonly DataContext _context;
        public WeatherForecastController(DataContext context)
        {
            _context = context;

        }
    

        [HttpGet]
        [AllowAnonymous]
       public async Task<IActionResult> GetValues()
       {
          var values=await _context.values.ToListAsync();
          return Ok(values);
       }

       [HttpGet("{id}")]
       [AllowAnonymous]
        public async Task<IActionResult> GetValues(int id)
       {
          var value=await _context.values.FirstOrDefaultAsync(x=>x.id==id);
          return Ok(value);
       }
    }
}
