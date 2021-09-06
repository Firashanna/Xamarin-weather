using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Weather.Models;

namespace Weather.Services
{
    //You replace this class witth your own Service from Project Part A
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "a0311119d3f08ce24bcd33ce20fbdf03"; // Your API Key

        //part of your event code here
        public async Task<Forecast> GetForecastAsync(string City)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            OnWeatherForecastAvailable("New Weather forecast for " + City + " available");

            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            OnWeatherForecastAvailable("New Weather forecast for (" + latitude.ToString() + " , " + longitude.ToString() + ") available"); ;

            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            using (HttpResponseMessage response = await httpClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    Forecast f = new Forecast();
                    WeatherApiData o = await response.Content.ReadFromJsonAsync<WeatherApiData>();
                    f.City = o.city.name;
                    f.Items = o.list.Select(x => new ForecastItem()
                    {
                        Temperature = x.main.temp,
                        DateTime = Convert.ToDateTime(x.dt_txt),
                        Description = x.weather[0].description
                    ,
                        Icon = x.weather[0].icon,
                        WindSpeed = x.wind.speed
                    }).ToList();
                    return f;



                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            return null;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public event EventHandler<string> WeatherForecastAvailable;


        protected virtual void OnWeatherForecastAvailable(string message)
        {
            EventHandler<String> handler = WeatherForecastAvailable;
            if (handler != null)
            {
                handler(this, message);
            }
        }
    }
}
