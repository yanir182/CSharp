using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        var myLocation = await GetMyLocation();
        if (myLocation != null)
        {
            double latitude = (double)myLocation["latitude"];
            double longitude = (double)myLocation["longitude"];
            Console.WriteLine($"координаты: Широта = {latitude}, Долгота = {longitude}");

            var nearbyCafe = await FindNearbycafe(latitude, longitude);
            if (nearbyCafe != null)
            {
                Console.WriteLine("Ближайшие кафе:");
                Console.WriteLine(nearbyCafe);
            }
            else
            {
                Console.WriteLine("Не удалось найти ближайшие кафе.");
            }
        }
        else
        {
            Console.WriteLine("где вы?");
        }
    }

    static async Task<JObject> GetMyLocation()
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync("https://yandex.ru/maps/11068/svetlograd/?ll=42.856628%2C45.328573&z=13");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(jsonString);
            }
            else
            {
                Console.WriteLine($"Ошибка получения местоположения: {response.StatusCode}");
                return null;
            }
        }
    }

    static async Task<JObject> FindNearbycafe(double latitude, double longitude)
    {
        string apiKey = "API";
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude},{longitude}&radius=5000&type=cafe&key={apiKey}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(jsonString);
            }
            else
            {
                Console.WriteLine($"Ошибка поиска кафе: {response.StatusCode}");
                return null;
            }
        }
    }
}