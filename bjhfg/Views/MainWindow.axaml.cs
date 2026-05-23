using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherAppAvalonia;

public partial class MainWindow : Window
{
   
    private readonly string apiKey = "api.openweathermap.org/data/2.5/forecast?id=524901&appid={API key}";
    
    // UI элементы (объявляем как поля класса для доступа из методов)
    private TextBox? CityTextBox;
    private Image? WeatherIcon;
    private TextBlock? CityLabel;
    private TextBlock? TemperatureLabel;
    private TextBlock? ConditionLabel;
    private TextBlock? HumidityLabel;
    private TextBlock? WindLabel;
    private Button? GetWeatherButton;

    public MainWindow()
    {
        InitializeComponent();  // Загружает XAML разметку
        FindControls();        // Находит все элементы управления
    }

    private void FindControls()
    {
       
        CityTextBox = this.FindControl<TextBox>("CityTextBox");
        WeatherIcon = this.FindControl<Image>("WeatherIcon");
        CityLabel = this.FindControl<TextBlock>("CityLabel");
        TemperatureLabel = this.FindControl<TextBlock>("TemperatureLabel");
        ConditionLabel = this.FindControl<TextBlock>("ConditionLabel");
        HumidityLabel = this.FindControl<TextBlock>("HumidityLabel");
        WindLabel = this.FindControl<TextBlock>("WindLabel");
        GetWeatherButton = this.FindControl<Button>("GetWeatherButton");

        //  Подписываемся на событие клика кнопки
        if (GetWeatherButton != null)
            GetWeatherButton.Click += async (s, e) => await GetWeather();
    }

    private async Task GetWeather()
    {
        string city = CityTextBox?.Text ?? "";
        
        // ✅ Проверка: город не должен быть пустым
        if (string.IsNullOrWhiteSpace(city))
        {
            await ShowMessage("Введите название города");
            return;
        }

        // 🔄 Блокируем кнопку на время загрузки (избегаем повторных кликов)
        if (GetWeatherButton != null)
        {
            GetWeatherButton.IsEnabled = false;
            GetWeatherButton.Content = "Загрузка...";
        }

        await LoadWeather(city);  // Асинхронный запрос к API

        // Разблокируем кнопку после загрузки
        if (GetWeatherButton != null)
        {
            GetWeatherButton.IsEnabled = true;
            GetWeatherButton.Content = "Узнать погоду";
        }
    }

    private async Task LoadWeather(string city)
    {
        try
        {
            using var client = new HttpClient(); // using = автоматическое освобождение ресурсов
            
            //  Формат URL
            // units=metric - температура в Цельсиях
         
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&lang=ru&appid={apiKey}";
            
            var response = await client.GetAsync(url);  // await = не блокируем UI

            if (response.IsSuccessStatusCode)  // 200-299 код успеха
            {
                string json = await response.Content.ReadAsStringAsync();
                ParseWeather(json);
            }
            else
            {
                await ShowMessage("Город не найден");
            }
        }
        catch (Exception ex)
        {
            // 📡 Обработка ошибок сети (нет интернета, таймаут и т.д.)
            await ShowMessage($"Ошибка: {ex.Message}");
        }
    }

    private void ParseWeather(string json)
    {
        // 📦 Парсим JSON ответ от сервера
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Извлекаем данные из JSON (обратите внимание на вложенность)
        string cityName = root.GetProperty("name").GetString() ?? "";
        double temp = root.GetProperty("main").GetProperty("temp").GetDouble();

        int humidity = root.GetProperty("main").GetProperty("humidity").GetInt32();
        double windSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble();
        string condition = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "";
        string iconCode = root.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "";

        // 🖥️ Обновляем UI (проверка на null обязательна)
        if (CityLabel != null) CityLabel.Text = $"📍 {cityName}";
        if (TemperatureLabel != null) TemperatureLabel.Text = $"{Math.Round(temp)}°C";
        if (ConditionLabel != null) ConditionLabel.Text = char.ToUpper(condition[0]) + condition.Substring(1);
        if (HumidityLabel != null) HumidityLabel.Text = $"💧 Влажность: {humidity}%";
        if (WindLabel != null) WindLabel.Text = $"💨 Ветер: {windSpeed} м/с";

        LoadIcon(iconCode);
    }

    private async void LoadIcon(string iconCode)
    {
        //  Загружаем иконку погоды с сервера OpenWeatherMap
        try
        {
            string url = $"http://openweathermap.org/img/w/{iconCode}.png";
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url);
            
            // Конвертируем байты в изображение
            using var stream = new System.IO.MemoryStream(bytes);
            var bitmap = new Bitmap(stream);
            
            if (WeatherIcon != null)
                WeatherIcon.Source = bitmap;
        }
        catch 
        { 
            // 🎨 Игнорируем ошибки загрузки иконки - погода всё равно покажется
        }
    }

    private async Task ShowMessage(string text)
    {
        // 💬 ВАЖНО: В Avalonia нет MessageBox, создаем диалог вручную
        var dialog = new Avalonia.Controls.Window
        {
            Title = "Информация",
            Content = new TextBlock { Text = text, Margin = new Avalonia.Thickness(20) },
            Width = 300,
            Height = 150
        };
        await dialog.ShowDialog(this);
    }
}
