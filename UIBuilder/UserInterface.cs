using System;
using Autodesk.Revit.Attributes;
using ArCm;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;

namespace UIBuilder
{
	public class UserInterface
	{
		public static void CreateUserInterface(UIControlledApplication application, bool isRelease)
		{
            string folderPath = @"C:\Users\79518\source\repos\RevitPlugin\RevitPlugin\bin\x64\Debug";
            if (isRelease == true)
            {
                folderPath = @"C:\Users\79518\source\repos\RevitPlugin\RevitPlugin\bin\x64\Release";
            }
            string folderImage = @"C:\Users\79518\source\repos\RevitPlugin\RevitPlugin\Images";

            string dllArCm = Path.Combine(folderPath, "ArCm.dll");
            string tabArCm = "АР";
            application.CreateRibbonTab(tabArCm);
            RibbonPanel panelTag = application.CreateRibbonPanel(tabArCm, "Tags");

            PushButton btnLayersTag = (PushButton)panelTag.AddItem(new PushButtonData("Layers tag", "Layers tag", dllArCm, typeof(ArCm.LayerTag).FullName));
            btnLayersTag.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTag32.png"), UriKind.Absolute));
            btnLayersTag.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTag16.png"), UriKind.Absolute));
            btnLayersTag.ToolTip = "Clicking at Wall/Floor/Roof and select the side for annotation of all layers of construction";
            btnLayersTag.LongDescription = "Не забудь нажать единожды при открытии модели кнопку TagAutoRefreshing, чтобы аннотации автоматически обновлялись при изменениях конструкций";

            PushButton btnRegisterLayerUpdater = (PushButton)panelTag.AddItem(new PushButtonData("RegisterLayerUpdater", "TagAutoRefreshing", dllArCm, typeof(ArCm.RegisterLayerUpdater).FullName));
            btnRegisterLayerUpdater.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "RegisterLayerUpdater32.png"), UriKind.Absolute));
            btnRegisterLayerUpdater.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "RegisterLayerUpdater16.png"), UriKind.Absolute));
            btnRegisterLayerUpdater.ToolTip = "Click once for auto updating of all layers tags";
            btnRegisterLayerUpdater.LongDescription = "Для автоматического обновления выносок надо нажать один раз при каждом открытии модели";

            PushButton btnLayerTagRefresh = (PushButton)panelTag.AddItem(new PushButtonData("Refresh Tag", "Refresh Tag", dllArCm, typeof(ArCm.LayerTagRefresh).FullName));
            btnLayerTagRefresh.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnLayerTagRefresh.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnLayerTagRefresh.ToolTip = "Update all layers tags";

            string dllStudy = Path.Combine(folderPath, "Study.dll");
            string tabStudy = "Study";
            application.CreateRibbonTab(tabStudy);
            RibbonPanel panelCommands = application.CreateRibbonPanel(tabStudy, "commands");

            PushButton btn1 = (PushButton)panelCommands.AddItem(new PushButtonData("Study", "Study", dllStudy, typeof(Study.Class1).FullName));
            btnLayerTagRefresh.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnLayerTagRefresh.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
        }
	}
}
