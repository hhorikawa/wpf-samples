using System;
using System.ComponentModel.DataAnnotations;

namespace RestWebApiServer
{

// Model
public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

    [Required(AllowEmptyStrings = true)] // 付けないと nullable になってしまう。
    public string Summary { get; set; }
} // class WeatherForecast

}
