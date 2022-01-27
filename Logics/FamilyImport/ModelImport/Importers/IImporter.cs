using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Logics.FamilyImport.ModelImport.Importers
{
    public interface IImporter
    {
		IDictionary Import();
    }
	public abstract class AbstractImporter<T> : IImporter
	{
		protected readonly string _json;


		protected AbstractImporter(string jsonFilePath)
		{
			_json = jsonFilePath;

		}

		public abstract Dictionary<string, T> ImportWork();
		public IDictionary Import()
		{
			return ImportWork();
		}

	}

}
