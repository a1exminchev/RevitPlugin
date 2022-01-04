using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Study
{
    public class RestSharper
    {
        string _adress;
        RestRequest _request;
        IRestResponse _response;
        RestClient _client;
        public RestSharper(string url)
        {
            _adress = url;
            _client = new RestClient(_adress);
        }
        public string Request { get; }
        public string Response { get; set; }
        public void Run()
        {
            _request = new RestRequest();
            _response = _client.Get(_request);
            Response = _response.Content;
        }
        private void Main(string[] args)
        {
            var request = new RestSharper("http://api.openweathermap.org/data/2.5/weather?q=Moscow&appid=fa72d984b737783c74e425cda2f273cd");

            request.Run();
            var response = request.Response;
            var json = JObject.Parse(response);
            var wind = json["wind"];

            Console.WriteLine(wind["speed"]);
            Console.ReadKey();
        }
    }
}
