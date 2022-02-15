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
	public class SweepImporter : AbstractImporter<SweepTransfer>
	{
		public SweepImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Id string and <SweepTransfer>s that has Method of creation

		public override Dictionary<string, SweepTransfer> ImportWork(FamilyDocumentData famDoc)
		{
			var dict = famDoc.Sweeps;
			return dict;
		}
	}
}
