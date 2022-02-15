using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using Logics.Export;
using Newtonsoft.Json;
using Logics.Import.Transforms;

namespace Logics.Import.ModelImport.Importers.Implementations
{
	public class SweptBlendImporter : AbstractImporter<SweptBlendTransfer>
	{
		public SweptBlendImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Id string and <SweptBlendTransfer>s that has Method of creation

		public override Dictionary<string, SweptBlendTransfer> ImportWork(FamilyDocumentData famDoc)
		{
			var dict = famDoc;
			return dict.SweptBlends;
		}
	}
}
