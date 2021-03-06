using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Logics.Export.ModelExport.Extractors.Implementations;

namespace Logics.Export.ModelExport.Extractors
{
	public interface IExtractor
	{
		IDictionary Extract();
	}
	public abstract class AbstractExtractor<T> : IExtractor
	{
		protected readonly Document _doc;


		protected AbstractExtractor(Document document)
		{
			_doc = document;

		}

		public abstract Dictionary<int, T> ExtractWork();
		public IDictionary Extract()
		{
			return ExtractWork();
		}

	}
}