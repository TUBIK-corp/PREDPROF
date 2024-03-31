using Modest.Helpers;
using Modest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Modest.Helpers
{
    public class Floor
    {
        public string Name { get; set; }
        public bool[] Lights { get; set; }
    }
    public class DataParser
    {
        public class NeededData
        {
            public List<Floor> Floors { get; set; }
            public int FloorCount { get; set; }
            public int RoomCount { get; set; }
        }

        public NeededData ParseFloors(JObject obj)
        {
            var dataObject = obj["message"]["windows"]!["data"];

            var result = new List<Floor>();

            var floorArray = dataObject!.Children().ToArray();
            int countFloors = floorArray.Length;
            int countLights = 0;

            foreach (var floor in floorArray)
            {
                var floorLights = floor.First()!.Children().ToArray();
                var floorLighted = new List<bool>();
                if (floor is JProperty property)
                {
                    string propertyName = property.Name;
                    foreach (var lights in floor.Children())
                    {
                        var JLights = lights.Cast<JValue>().ToArray();
                        if (countLights == 0)
                            countLights = JLights.Length;
                        foreach (bool light in JLights)
                        {
                            floorLighted.Add(light);
                        }
                    }
                    result.Add(new Floor()
                    {
                        Name = propertyName,
                        Lights = floorLighted.ToArray()
                    });
                }
            }

            result = result.OrderBy(x => Convert.ToInt32(x.Name.Split("_").Last())).ToList(); ;
            var needed = new NeededData()
            {
                Floors = result,
                FloorCount = countFloors,
                RoomCount = countLights
            };
            return needed;
        }
    }

    public class SuperParse
    {
        public class Data
        {
            public int count { get; set; }
            public List<int> rooms { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public string date { get; set; }
        }
        public class ViewData
        {
            public Root root { get; set;}
            public int[] rooms { get; set; }
        }

        public ViewData Parse(string json)
        {
            JObject jsonObj = JObject.Parse(json);
            var jsonObject = jsonObj["message"];

            int roomsCount = jsonObject["flats_count"]["data"].Value<int>();
            JArray windowsForRoom = (JArray)jsonObject["windows_for_flat"]["data"];
            JObject windowsData = (JObject)jsonObject["windows"]["data"];

            List<int> litRooms = new List<int>();

            for (int floor = 1; floor <= windowsData.Count; floor++)
            {
                int shift = 0;
                for (int room = 1; room <= roomsCount; room++)
                {
                    int windowCount = windowsForRoom[room - 1].Value<int>();
                    bool isLit = false;

                    for (int window = 0; window < windowCount; window++)
                    {
                        if (windowsData[$"floor_{floor}"][window + shift].Value<bool>())
                        {
                            isLit = true;
                            break;
                        }
                    }
                    if (isLit)
                    {
                        litRooms.Add((floor - 1) * roomsCount + room);
                    }
                    shift += windowCount;
                }
            }

            string date = UnixTimeStampToDateTime(jsonObject["date"]["data"].Value<long>()).ToString("dd.MM.yyyy");

            JObject response = new JObject
            {
                ["data"] = new JObject
                {
                    ["count"] = litRooms.Count,
                    ["rooms"] = new JArray(litRooms)
                },
                ["date"] = date
            };
            var jsonResult = response.ToString();

            var answer = JsonConvert.DeserializeObject<Root>(jsonResult);
            var view = new ViewData();
            view.rooms = windowsForRoom.Values<int>().ToArray();
            view.root = answer;
            return view;
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime unixStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateTime = unixStartDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
