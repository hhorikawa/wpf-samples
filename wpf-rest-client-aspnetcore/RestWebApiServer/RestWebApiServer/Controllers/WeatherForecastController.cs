using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestWebApiServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestWebApiServer.Controllers
{

// ASP.NET Core Web APIでは ControllerBase クラスから派生させる。基底クラスが ASP.NET Web API 2 の ApiController とは異なる。
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    static readonly string[] Summaries = new[] {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    // 動詞を指定.
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
    }
} // class WeatherForecastController

}
