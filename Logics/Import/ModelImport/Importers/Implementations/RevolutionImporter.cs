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
	public class RevolutionImporter : AbstractImporter<RevolutionTransfer>
	{
		public RevolutionImporter(string jsonFilePath) : base(jsonFilePath) {
		}   //taking for constructor json from base for doing ImportWork() where it converts text to dictionary of Id string and <RevolutionTransfer>s that has Method of creation

		public override Dictionary<string, RevolutionTransfer> ImportWork(FamilyDocumentData Data)
		{
			var dict = Data.Revolutions;
			return dict;
		}
		public override Dictionary<string, RevolutionTransfer> ImportWork(ProjectDocumentData Data)
		{
			return null;
		}
	}
}
