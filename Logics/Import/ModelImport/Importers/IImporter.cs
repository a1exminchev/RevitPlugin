using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Logics.Import.ModelImport.Importers
{
    public interface IImporter
    {
		IDictionary Import(FamilyDocumentData Data);
		IDictionary Import(ProjectDocumentData Data);
	}
	public abstract class AbstractImporter<T> : IImporter
	{
		protected readonly string _json;
		protected AbstractImporter(string jsonFilePath)
		{
			_json = jsonFilePath;
		}

		public abstract Dictionary<string, T> ImportWork(FamilyDocumentData Data);
		public abstract Dictionary<string, T> ImportWork(ProjectDocumentData Data);
		public IDictionary Import(FamilyDocumentData Data)
		{
			return ImportWork(Data);
		}
		public IDictionary Import(ProjectDocumentData Data)
		{
			return ImportWork(Data);
		}
	}
}
