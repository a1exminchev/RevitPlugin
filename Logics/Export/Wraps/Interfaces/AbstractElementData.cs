using Autodesk.Revit.DB;
using System.Linq;
using System;
using Autodesk.Revit.DB.ExtensibleStorage;
using Logics.Export.Wraps.Interfaces;

namespace Logics.Export.Wraps.Implementations{
	public abstract class AbstractElementData : IElement
	{
		public Document doc { get; set; }
		public int             Id       { get; set; }
		//public int			   OldId	{ get; set; }
		public string          Name     { get; set; }
		public string          Type     { get; set; }
		public bool			   isPinned { get; set; }
		public string		   Category { get; set; }
		public AbstractElementData() {
		}

		public AbstractElementData(Element el) {
			Id = el.Id.IntegerValue;
			Name = el.Name;
			Type = el.GetType().ToString();
			isPinned = el.Pinned;
			Category = el.Category?.ToString();

			string schemaName = "OldDocData";
			Schema mySchema = Schema.ListSchemas().FirstOrDefault(x => x.SchemaName == schemaName);
			if (mySchema == null)
            {
				Guid guid = new Guid("681AB6EA-AE56-41A1-A880-B3918B417B0F");
				SchemaBuilder sb = new SchemaBuilder(guid);
				sb.SetSchemaName(schemaName);
				sb.AddSimpleField("OldId", typeof(int));
				mySchema = sb.Finish();
            }
			Entity entity = new Entity(mySchema);
			Field oldIdField = mySchema.GetField("OldId");
		}
	}
}