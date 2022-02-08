using System;
using Autodesk.Revit.UI;
using PluginUtil;
using PluginUtil.Loger;
using UIBuilder;
using System.IO;
using PluginUtils;

namespace RevitPlugin
{
    public class App : IExternalApplication
    {
        
        public Result OnStartup(UIControlledApplication application)
        {
            var result = Result.Succeeded;
            try
            {
                GlobalData.UiCApp = application;
                ReferenceResolver.ReloadReference();

                bool isRelease = false;

                UserInterface.CreateUserInterface(application, isRelease);
                
            }
            catch (Exception e)
            {
                e.LogError();
                // result = Result.Failed;
            }

            return result;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            var result = Result.Succeeded;


            return result;
        }
    }
}