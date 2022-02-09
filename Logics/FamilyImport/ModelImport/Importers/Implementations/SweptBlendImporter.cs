﻿using System.Collections;
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
	public class SweptBlendImporter : AbstractImporter<SweptBlendTransfer>
	{
		public SweptBlendImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Id string and <SweptBlendTransfer>s that has Method of creation

		public override Dictionary<string, SweptBlendTransfer> ImportWork()
		{
			var famDict = JsonConvert.DeserializeObject<FamilyDocumentData>(_json);
			var dict = famDict;
			return dict.SweptBlends;
		}
	}
}