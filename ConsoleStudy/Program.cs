using System;
using System.IO;
using StudyHome;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Logics.FamilyExport.ModelExport;
using RestSharp;
using Newtonsoft.Json;
using Logics.FamilyImport;
using System.Reflection;
using Logics.FamilyImport.ModelImport.Importers;
using PluginUtil;
using PluginLogics;
using Logics.FamilyImport.Transforms;
using Autodesk.Revit.DB;

namespace ConsoleStudy
{
    class Program
    {
        static void Main(string[] args) {
            //var person = new Person { Age = 24, Name = "Aleksey" };
            //string jsonData = JsonConvert.SerializeObject(person);
            //var personClon = JsonConvert.DeserializeObject<Person>(jsonData);
            //var obj = JsonConvert.DeserializeObject<Dictionary<string, Person>>(File.ReadAllText(@"C:\Users\Aleksey Minchev\source\repos\RevitPlugin\StudyHome\PersJson.json"));

            //string file = File.ReadAllText(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + @"\StudyTask\Files\FamilyData.json");
            //var obj = JsonConvert.DeserializeObject<Dictionary<string, ExtrusionTransform>>(file);
            //Console.WriteLine(obj.First().Value.EndOffset);
            //Console.ReadKey();

            string r = "Array43PointOfLine6";
            
            Console.WriteLine(r.Split('e')[1]);
            Console.WriteLine(r.Split('y')[1].Split('P')[0]);
            Console.ReadKey();

        }
    }
}
