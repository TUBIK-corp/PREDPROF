using Modest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Modest.Helpers
{
    public static class HttpHelper
    {
        public async static Task<(string, JObject)> GetRooms(string parameters)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Auth-Token", "ppo_10_17129"); // указываем логин в заголовке

            var response = await client.GetAsync("https://olimp.miet.ru/ppo_it_final" + parameters); // отправляем GET-запрос

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JObject.Parse(responseContent);
                return (responseContent, token);
            }
            else
            {
                Console.WriteLine("Ошибка: " + response.StatusCode);
            }
            return ("", null);
        }
        public async static Task<Rootobject> GetDates(string parameters)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Auth-Token", "ppo_10_17129"); 

            var response = await client.GetAsync("https://olimp.miet.ru/ppo_it_final" + parameters); 

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<Rootobject>(responseContent);
                return token;
            }
            else
            {
                Console.WriteLine("Ошибка: " + response.StatusCode);
            }
            return null;
        }
    }
}
