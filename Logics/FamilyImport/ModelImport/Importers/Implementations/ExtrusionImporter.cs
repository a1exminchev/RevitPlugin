using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using Logics.FamilyExport;
using Newtonsoft.Json;
using Logics.FamilyImport.Transforms;

namespace Logics.FamilyImport.ModelImport.Importers.Implementations
{
	public class ExtrusionImporter : AbstractImporter<ExtrusionTransform>
	{
		public ExtrusionImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Name+Id string and <ExtrusionTransform>s that has Method of creation

		public override Dictionary<string, ExtrusionTransform> ImportWork()
		{
			var dict = new Dictionary<string, ExtrusionTransform>();
			dict = JsonConvert.DeserializeObject<Dictionary<string, ExtrusionTransform>>(_json);
			return dict;
		}
	}
}
