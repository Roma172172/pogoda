using System;
using System.Threading.Tasks;
using WeatherAppAvalonia.Models;
using WeatherAppAvalonia.Services;

namespace WeatherAppAvalonia.ViewModels;

// Главная ViewModel (связывает UI и данные)
public class MainWindowViewModel
{
    private readonly WeatherApiService _weatherService = new();
    
    // Свойства для привязки к UI
    public string CityName { get; set; } = "";
    public string Temperature { get; set; } = "--°C";
    public string Humidity { get; set; } = "💧 Влажность: --%";
    public string Wind { get; set; } = "💨 Ветер: -- м/с";
    public string Condition { get; set; } = "---";
    public string IconUrl { get; set; } = "";
    public bool IsLoading { get; set; } = false;

    public async Task LoadWeather(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return;

        IsLoading = true;
        
        var data = await _weatherService.GetWeatherAsync(city);
        
        if (data != null)
        {
            // Обновляем UI через свойства
            CityName = $"📍 {data.CityName}";
            Temperature = $"{Math.Round(data.Temperature)}°C";
            Humidity = $"💧 Влажность: {data.Humidity}%";
            Wind = $"💨 Ветер: {data.WindSpeed} м/с";
            Condition = char.ToUpper(data.Condition[0]) + data.Condition.Substring(1);
            IconUrl = $"http://openweathermap.org/img/w/{data.IconCode}.png";
        }
        
        IsLoading = false;
    }
}
