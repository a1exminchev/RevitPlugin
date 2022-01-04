using System;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;


namespace Functions
{
	public class ParameterOperations
	{
		public static void CreateProjectParameterFromExistingSharedParameter(Application app, string name, CategorySet cats, BuiltInParameterGroup group, bool inst)
		{
			DefinitionFile defFile = app.OpenSharedParameterFile();
			if (defFile == null) throw new Exception("No SharedParameter File!");

			var v = (from DefinitionGroup dg in defFile.Groups
					 from ExternalDefinition d in dg.Definitions
					 where d.Name == name
					 select d);
			if (v == null || v.Count() < 1) throw new Exception("Invalid Name Input!");

			ExternalDefinition def = v.First();
			
			Autodesk.Revit.DB.Binding binding = app.Create.NewTypeBinding(cats);
			if (inst) binding = app.Create.NewInstanceBinding(cats);

			BindingMap map = (new UIApplication(app)).ActiveUIDocument.Document.ParameterBindings;
			map.Insert(def, binding, group);
		}


	}
}
