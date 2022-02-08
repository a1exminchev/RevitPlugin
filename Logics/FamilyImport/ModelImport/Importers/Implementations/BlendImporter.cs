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
	public class BlendImporter : AbstractImporter<BlendTransfer>
	{
		public BlendImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Id string and <BlendTransfer>s that has Method of creation

		public override Dictionary<string, BlendTransfer> ImportWork()
		{
			var famDict = JsonConvert.DeserializeObject<FamilyDocumentData>(_json);
			var dict = famDict.Blends;
			return dict;
		}
	}
}
