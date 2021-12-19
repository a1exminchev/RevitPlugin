using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Study
{
    class jsOn
    {
        public void Main()
        {
            var person = new Person { Age = 24, Name = "Aleksey" };
            string jsonData = JsonConvert.SerializeObject(person);
            var personClon = JsonConvert.DeserializeObject<Person>(jsonData);

            var js = JsonConvert.DeserializeObject<Person>(File.ReadAllText(@"C:\Users\79518\source\repos\RevitPlugin\Study\PersJson.json"));
        }
    }
}
