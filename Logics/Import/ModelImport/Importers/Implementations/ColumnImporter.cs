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
	public class ColumnImporter : AbstractImporter<ColumnTransfer>
	{
		public ColumnImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Id string and <ColumnTransfer>s that has Method of creation

		public override Dictionary<string, ColumnTransfer> ImportWork(FamilyDocumentData Data)
		{
			return null;
		}
		public override Dictionary<string, ColumnTransfer> ImportWork(ProjectDocumentData Data)
		{
			var dict = Data.Columns;
			return dict;
		}
	}
}
