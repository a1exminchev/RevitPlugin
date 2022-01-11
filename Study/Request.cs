using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace StudyHome
{
    public class Request
    {
        HttpWebRequest _request;
        string _adress;
        public string Response { get; set; }
        public Request(string adress)
        {
            _adress = adress;
        }
        public void Run()
        {
            _request = (HttpWebRequest)WebRequest.Create(_adress);
            _request.Method = "Get";
            try
            {
                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                var stream = response.GetResponseStream();
                if (stream != null) Response = new StreamReader(stream).ReadToEnd();
            }
            catch{ }
        }
        private void Main()
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
