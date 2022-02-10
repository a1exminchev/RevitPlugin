using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Logics.FamilyImport.ModelImport.Importers
{
    public interface IImporter
    {
		IDictionary Import(FamilyDocumentData famDoc);
    }
	public abstract class AbstractImporter<T> : IImporter
	{
		protected readonly string _json;


		protected AbstractImporter(string jsonFilePath)
		{
			_json = jsonFilePath;

		}

		public abstract Dictionary<string, T> ImportWork(FamilyDocumentData famDoc);
		public IDictionary Import(FamilyDocumentData famDoc)
		{
			return ImportWork(famDoc);
		}

	}

}
