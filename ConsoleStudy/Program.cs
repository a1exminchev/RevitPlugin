using System;
using System.IO;
using Study;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ConsoleStudy
{
    class Program
    {
        static void Main(string[] args)
        {
            var request = new Request("http://api.openweathermap.org/data/2.5/weather?q=Moscow&appid=fa72d984b737783c74e425cda2f273cd");
            request.Run();
            var response = request.Response;
            var json = JObject.Parse(response);
            var wind = json["wind"];

            Console.WriteLine(wind["speed"]);
            Console.ReadKey();
        }
        
    }
}
