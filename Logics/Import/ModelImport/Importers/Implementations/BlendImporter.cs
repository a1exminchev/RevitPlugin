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
	public class BlendImporter : AbstractImporter<BlendTransfer>
	{
		public BlendImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Id string and <BlendTransfer>s that has Method of creation

		public override Dictionary<string, BlendTransfer> ImportWork(FamilyDocumentData Data)
		{
			var dict = Data.Blends;
			return dict;
		}
		public override Dictionary<string, BlendTransfer> ImportWork(ProjectDocumentData Data)
		{
			return null;
		}
	}
}
